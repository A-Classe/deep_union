using System;
using System.Collections.Generic;
using AnimationPro.RunTime;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Core.Utility.UI.Navigation
{
    public class Navigation<T>: ITickable
    {
        private readonly Dictionary<T, UIManager> managers;
        private UIManager current;
        private T currentNav;
        
        private readonly InputEvent cancelEvent;
        private readonly InputEvent clickEvent;
        private readonly InputEvent moveEvent;
        
        private float currentTime;
        private float initialInterval = StartInterval;

        public event Action<InputAction.CallbackContext> OnCancel;

        private bool isActive;
        
        public bool IsActive => isActive;

        /// <summary>
        ///     押し続けるたびに減らす感覚
        /// </summary>
        private const float IntervalIncrement = 0.05f;

        /// <summary>
        ///     初回の呼び出し感覚
        /// </summary>
        private const float StartInterval = 0.6f;

        /// <summary>
        ///     最小の呼び出し感覚
        /// </summary>
        private readonly float minInterval = 0.1f;

        public Navigation(Dictionary<T, UIManager> initialManagers)
        {
            managers = initialManagers;
            
            moveEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Move);
            moveEvent.Started += OnMoveStarted;
            moveEvent.Canceled += OnMoveCanceled;

            clickEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Click);
            clickEvent.Started += OnClick;

            cancelEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Cancel);
            cancelEvent.Canceled += ctx =>
            {
                if (!isActive) return;
                OnCancel?.Invoke(ctx);
            };
        }

        public void Tick()
        {
            var moveValue = moveEvent.ReadValue<Vector2>();
            if (Math.Abs(moveValue.y) > 0.05f || Math.Abs(moveValue.x) > 0.05f) OnMove(moveValue);
        }

        public void SetScreen(T nav, bool isAnimate = true, bool isReset = false)
        {
            if (!isActive) return;
            
            if (current == null)
            {
                NavigateWith(nav);
                return;
            }

            if (isAnimate)
            {
                current.Finished(current.GetContext().FadeOut(Easings.Default(0.3f)), () =>
                {
                    NavigateWith(nav);
                });
            }
            else
            {
                NavigateWith(nav);
            }

            void NavigateWith(T n)
            {
                current = managers[n];
                currentNav = n;
                if (current == null) throw new NotImplementedException();
                if (isAnimate)
                {
                    current.Initialized(current.GetContext().FadeIn(Easings.Default(0.3f)), isReset);
                }
                else
                {
                    current.Initialized(current.GetContext().SlideTo(new Vector2(0,0), Easings.Default(0.1f)), isReset);
                }
            }
        }
        
        private void OnMove(Vector2 input)
        {
            if (!isActive) return;

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
            if (!isActive) return;

            var input = context.ReadValue<Vector2>();
            if (current != null) current.Select(input);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (!isActive) return;

            initialInterval = StartInterval;
            currentTime = 0f;
        }

        private void OnClick(InputAction.CallbackContext _)
        {
            if (!isActive) return;

            if (current != null) current.Clicked();
        }

        public T GetCurrentNav() => currentNav;

        public void SetActive(bool value) => isActive = value;

    }
}