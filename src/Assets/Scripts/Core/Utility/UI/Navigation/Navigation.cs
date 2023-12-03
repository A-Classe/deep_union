using System;
using System.Collections.Generic;
using AnimationPro.RunTime;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Core.Utility.UI.Navigation
{
    public class Navigation<T> : ITickable
    {
        /// <summary>
        ///     押し続けるたびに減らす感覚
        /// </summary>
        private const float IntervalIncrement = 0.08f;

        /// <summary>
        ///     初回の呼び出し感覚
        /// </summary>
        private const float StartInterval = 0.5f;

        private readonly InputEvent cancelEvent;
        private readonly InputEvent clickEvent;
        private readonly Dictionary<T, UIManager> managers;

        /// <summary>
        ///     最小の呼び出し間隔
        /// </summary>
        private readonly float minInterval = 0.1f;

        private readonly InputEvent moveEvent;
        private UIManager current;
        private T currentNav;

        private float currentTime;
        private float initialInterval = StartInterval;

        public Navigation(Dictionary<T, UIManager> initialManagers)
        {
            managers = initialManagers;

            moveEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Move);
            moveEvent.Started += OnMoveStarted;
            moveEvent.Canceled += OnMoveCanceled;

            clickEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Click);
            clickEvent.Started += OnClick;

            cancelEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Cancel);
            cancelEvent.Started += ctx =>
            {
                if (!IsActive) return;
                OnCancel?.Invoke(ctx);
            };
        }

        public bool IsActive { get; private set; }

        public void Tick()
        {
            var moveValue = moveEvent.ReadValue<Vector2>();
            if (Math.Abs(moveValue.y) > 0.05f || Math.Abs(moveValue.x) > 0.05f) OnMove(moveValue);
        }

        public event Action<InputAction.CallbackContext> OnCancel;

        public void SetScreen(T nav, bool isAnimate = true, bool isReset = false)
        {
            
            if (!IsActive) return;

            if (current == null)
            {
                NavigateWith(nav);
                return;
            }

            if (isAnimate)
                current.Finished(current.GetContext().FadeOut(Easings.Default(0.3f)), () => { NavigateWith(nav); });
            else
                current.Finished(current.GetContext().SlideTo(new Vector2(0f, 0f), Easings.Default(0.1f)), () => { NavigateWith(nav); });

            void NavigateWith(T n)
            {
                current = managers[n];
                currentNav = n;
                if (current == null) throw new NotImplementedException(n.ToString());
                if (isAnimate)
                    current.Initialized(current.GetContext().FadeIn(Easings.Default(0.3f)), isReset);
                else
                    current.Initialized(current.GetContext().SlideTo(new Vector2(0, 0), Easings.Default(0.1f)),
                        isReset);
            }
        }

        private void OnMove(Vector2 input)
        {
            if (!IsActive) return;
   
            currentTime += Time.deltaTime;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (initialInterval == StartInterval && currentTime == 0f && current != null) current.Select(input);
            if (currentTime < initialInterval) return; // 一定の間隔が経過していない場合、何もしない

            // 呼び出し間隔を増加させる
            initialInterval = Math.Max(initialInterval -= IntervalIncrement, minInterval);

            if (current != null) current.Select(input);
            currentTime = 0f;
        }

        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            if (!IsActive) return;

            var input = context.ReadValue<Vector2>();
            if (current != null) current.Select(input);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (!IsActive) return;

            initialInterval = StartInterval;
            currentTime = 0f;
        }

        private void OnClick(InputAction.CallbackContext _)
        {
            if (!IsActive) return;

            if (current != null) current.Clicked();
        }

        public T GetCurrentNav()
        {
            return currentNav;
        }

        public void SetActive(bool value)
        {
            IsActive = value;
        }
    }
}