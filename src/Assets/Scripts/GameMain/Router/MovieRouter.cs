using Core.Input;
using Core.Scenes;
using Core.Utility.UI.Component;
using Module.Extension.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Router
{
    public class MovieRouter : IStartable, ITickable
    {
        private readonly VideoPlayerController videoController;

        private readonly SceneChanger sceneChanger;

        private readonly HoldVisibleObject holdVisual;

        private readonly InputEvent anyKeyEvent;

        [Inject]
        public MovieRouter(
            SceneChanger sceneChanger,
            VideoPlayerController videoController,
            HoldVisibleObject holdVisual
        )
        {
            this.sceneChanger = sceneChanger;
            this.videoController = videoController;

            this.holdVisual = holdVisual;
            this.holdVisual.OnHoldFinished += ChangeNextScene;

            anyKeyEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.UI.AnyKey);
        }

        public void Start()
        {
            videoController.Play(ChangeNextScene);
        }

        private void ChangeNextScene()
        {
            sceneChanger.Next();
        }

        public void Tick()
        {
            float value = anyKeyEvent.ReadValue<float>();
            if (holdVisual.IsVisible == value > 0 && holdVisual.IsVisible)
            {
                holdVisual.UpdateHoldTime(Time.deltaTime);
            }

            holdVisual.SetVisible(value > 0);
        }
    }
}