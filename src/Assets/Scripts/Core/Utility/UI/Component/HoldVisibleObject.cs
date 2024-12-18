using System;
using AnimationPro.RunTime;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Utility.UI.Component
{
    public class HoldVisibleObject : SwitchableAnimationBehaviour
    {
        [SerializeField] private Slider visualSlider;
        public override ContentTransform EnterTransform { get; set; }
        public override ContentTransform ExitTransform { get; set; }

        private readonly float ExitDelaySec = 0.3f;
        private readonly float EnterDelaySec = 0.6f;

        public bool IsVisible => !State;

        private readonly float holdFinishedTimeSec = 2.0f;
        private float currentTime;

        public event Action OnHoldFinished;

        private void Start()
        {
            EnterTransform = this.FadeOut(Easings.QuartIn(EnterDelaySec));
            ExitTransform = this.FadeIn(Easings.Default(ExitDelaySec));
            visualSlider.minValue = 0.0f;
            visualSlider.maxValue = 1.0f;
            State = true;
        }

        public void SetVisible(bool isVisible)
        {
            if (State != !isVisible)
            {
                currentTime = 0f;
            }

            State = !isVisible;
        }

        public void UpdateHoldTime(float delta)
        {
            if (!IsVisible)
            {
                return;
            }

            currentTime += delta;
            UpdateSlider();
        }

        private void UpdateSlider()
        {
            visualSlider.value = currentTime <= 0.0f ? 0 : currentTime / holdFinishedTimeSec;

            if (currentTime > holdFinishedTimeSec)
            {
                OnHoldFinished?.Invoke();
            }
        }
    }
}