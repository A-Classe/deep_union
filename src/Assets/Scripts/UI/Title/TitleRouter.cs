using System;
using System.Collections.Generic;
using AnimationPro.RunTime;
using Core.Input;
using JetBrains.Annotations;
using UI.Title.Credit;
using UI.Title.Option1;
using UI.Title.Option2;
using UI.Title.Option3;
using UI.Title.Option4;
using UI.Title.Quit;
using UI.Title.StageSelect;
using UI.Title.Title;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace UI.Title
{
    internal class TitleRouter : IStartable, ITickable
    {
        /// <summary>
        ///     押し続けるたびに減らす感覚
        /// </summary>
        private const float IntervalIncrement = 0.05f;

        /// <summary>
        ///     初回の呼び出し感覚
        /// </summary>
        private const float StartInterval = 0.6f;

        private readonly CreditManager credit;

        private readonly Dictionary<Nav, IUIManager> managers = new();

        /// <summary>
        ///     最小の呼び出し感覚
        /// </summary>
        private readonly float minInterval = 0.1f;

        private readonly Option1Manager option1;
        private readonly Option2Manager option2;
        private readonly Option3Manager option3;
        private readonly Option4Manager option4;
        private readonly QuitManager quit;
        private readonly StageSelectManager stageSelect;
        private readonly TitleManager title;
        private InputEvent cancelEvent;
        private InputEvent clickEvent;

        [CanBeNull] private IUIManager current;
        private Nav? currentNav;

        private float currentTime;
        private float initialInterval = StartInterval;

        private InputEvent moveEvent;

        [Inject]
        public TitleRouter(
            TitleManager titleManager,
            QuitManager quitManager,
            Option1Manager option1Manager,
            Option2Manager option2Manager,
            Option3Manager option3Manager,
            Option4Manager option4Manager,
            CreditManager creditManager,
            StageSelectManager stageSelectManager
        )
        {
            title = titleManager;
            quit = quitManager;
            option1 = option1Manager;
            option2 = option2Manager;
            option3 = option3Manager;
            option4 = option4Manager;
            credit = creditManager;
            stageSelect = stageSelectManager;
        }


        public void Start()
        {
            moveEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Move);
            moveEvent.Started += OnMoveStarted;
            moveEvent.Canceled += OnMoveCanceled;

            clickEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Click);
            clickEvent.Started += OnClick;

            cancelEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Cancel);
            cancelEvent.Started += OnCanceled;

            SetNavigation();
            managers.Clear();
            managers.Add(Nav.Title, title);
            managers.Add(Nav.Quit, quit);
            managers.Add(Nav.Option1, option1);
            managers.Add(Nav.Option2, option2);
            managers.Add(Nav.Option3, option3);
            managers.Add(Nav.Option4, option4);
            managers.Add(Nav.Credit, credit);
            managers.Add(Nav.StageSelect, stageSelect);

            SetScreen(Nav.Title);
        }

        public void Tick()
        {
            var moveValue = moveEvent.ReadValue<Vector2>();
            if (Math.Abs(moveValue.y) > 0.05f || Math.Abs(moveValue.x) > 0.05f) OnMove(moveValue);
        }


        private void OnMove(Vector2 input)
        {
            currentTime += Time.deltaTime;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (initialInterval == StartInterval && currentTime == 0f) current?.Select(input);
            if (currentTime < initialInterval) return; // 一定の間隔が経過していない場合、何もしない

            // 呼び出し間隔を増加させる
            initialInterval = Math.Max(initialInterval -= IntervalIncrement, minInterval);

            current?.Select(input);
            currentTime = 0f;
        }

        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            current?.Select(input);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            initialInterval = StartInterval;
            currentTime = 0f;
        }

        private void OnClick(InputAction.CallbackContext _)
        {
            current?.Clicked();
        }

        private void SetNavigation()
        {
            title.onQuit = () => SetScreen(Nav.Quit);
            title.onOption = () => SetScreen(Nav.Option1);
            title.onCredit = () => SetScreen(Nav.Credit);
            title.onStart = () => SetScreen(Nav.StageSelect);

            quit.onClick = val =>
            {
                if (val)
                {
                }
                else
                {
                    NavigateToTitle();
                }
            };
            option1.onClick = Opt1Clicked;
            option1.onBack = NavigateToTitle;

            option2.onFullScreen = isOn => { Debug.Log("onFullScreen : " + isOn); };
            option2.onBrightness = val => { Debug.Log("onBrightness : " + val); };
            option2.onBack = NavigateToOption1;

            option3.onVolumeChanged = Opt3ParameterChanged;
            option3.onBack = NavigateToOption1;
            //TODO  option4

            //TODO  credit

            stageSelect.onStage = StageSelected;
            stageSelect.onBack = NavigateToTitle;
        }

        private void StageSelected(StageSelectManager.Nav nav)
        {
            Debug.Log("StageSelected : " + nav);
        }

        private void Opt3ParameterChanged(Option3Manager.Nav nav, float value)
        {
            Debug.Log("volumeChanged : " + nav + "><" + value);
        }

        private void NavigateToTitle()
        {
            SetScreen(Nav.Title);
        }

        private void NavigateToOption1()
        {
            SetScreen(Nav.Option1);
        }

        private void Opt1Clicked(Option1Manager.Nav nav)
        {
            switch (nav)
            {
                case Option1Manager.Nav.Video:
                    SetScreen(Nav.Option2);
                    break;
                case Option1Manager.Nav.Audio:
                    SetScreen(Nav.Option3);
                    break;
                case Option1Manager.Nav.KeyConfig:
                    SetScreen(Nav.Option4);
                    break;
            }
        }

        private void OnCanceled(InputAction.CallbackContext context)
        {
            switch (currentNav)
            {
                case Nav.Option2:
                case Nav.Option3:
                case Nav.Option4:
                    NavigateToOption1();
                    break;
                default:
                    NavigateToTitle();
                    break;
            }
        }

        private void SetScreen(Nav nav)
        {
            if (current == null)
            {
                current = managers[nav];
                currentNav = nav;
                if (current == null) throw new NotImplementedException();

                current?.Initialized(current.GetContext().FadeIn(Easings.Default(0.3f)));
                return;
            }

            current?.Finished(current.GetContext().FadeOut(Easings.Default(0.3f)), () =>
            {
                current = managers[nav];
                currentNav = nav;
                if (current == null) throw new NotImplementedException();

                current?.Initialized(current.GetContext().FadeIn(Easings.Default(0.3f)));
            });
        }

        private enum Nav
        {
            Title,
            Option1,
            Option2,
            Option3,
            Option4,
            Quit,
            Credit,
            StageSelect
        }
    }
}