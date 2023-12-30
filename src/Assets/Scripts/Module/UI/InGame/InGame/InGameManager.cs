using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Component;
using Core.Utility.UI.Navigation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.UI.InGame.InGame
{
    public class InGameManager : UIManager
    {
        // hp
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI maxHp;
        [SerializeField] private Slider sliderHp;
        [SerializeField] private Image gaugeHpBackground;
        [SerializeField] private Image handleBackground;

        // progress
        [SerializeField] private TextMeshProUGUI currentProgress;

        // resource
        [SerializeField] private PopupText currentRes;

        // workers
        [SerializeField] private PopupText currentWorkers;
        [SerializeField] private AnimateObject workersIcon;
        [SerializeField] private float startOffsetY; // -205
        [SerializeField] private float endOffsetY; // -37

        // color
        // x:Hue, y:Saturation, z:Value
        private readonly Vector3 maxHpHSV = new(109 / 360f, 1f, 1f);
        private readonly Vector3 minHpHSV = new(0f, 1f, 0.74f);
        private uint? maxHpValue;
        private uint? maxResValue;
        private uint? maxWorkersValue;

        public override void Select(Vector2 direction)
        {
            // NOP.
        }

        public override void Clicked()
        {
            // NOP.
        }

        public void SetHp(uint current, uint? max = null)
        {
            if (max.HasValue)
            {
                maxHpValue = max.Value;
                maxHp.text = max.Value.ToString();
                sliderHp.maxValue = max.Value;
                sliderHp.minValue = 0;
            }

            if (!maxHpValue.HasValue)
            {
                throw new Exception();
            }

            if (current > maxHpValue.Value)
            {
                return;
            }

            currentHp.text = current.ToString();
            sliderHp.value = current;
            var currentValue = current / (float)maxHpValue;

            if (gaugeHpBackground != null)
            {
                gaugeHpBackground.color = ChangeColor(minHpHSV, maxHpHSV, currentValue);
            }

            if (handleBackground != null)
            {
                handleBackground.color = ChangeColor(minHpHSV, maxHpHSV, currentValue);
            }
        }

        private Color ChangeColor(Vector3 min, Vector3 max, float value)
        {
            Vector3 lifeColorHSV = new(min.x + (max.x - min.x) * value, min.y + (max.y - min.y) * value,
                min.z + (max.z - min.z) * value);
            var color = Color.HSVToRGB(lifeColorHSV.x, lifeColorHSV.y, lifeColorHSV.z);
            return color;
        }

        public void SetStageProgress(uint value)
        {
            currentProgress.text = value.ToString();
        }

        public void SetResource(uint current, uint? max = null)
        {
            if (max.HasValue)
            {
                maxResValue = max.Value;
            }

            if (!maxResValue.HasValue)
            {
                throw new Exception();
            }

            if (current > maxResValue.Value)
            {
                return;
            }

            currentRes.SetTextIsUp(current.ToString());
        }

        public void SetWorkerCount(uint value, uint? max = null)
        {
            if (max.HasValue)
            {
                maxWorkersValue = max.Value;
            }

            if (!maxWorkersValue.HasValue)
            {
                throw new Exception();
            }

            if (value > maxWorkersValue.Value)
            {
                return;
            }

            currentWorkers.SetTextIsUp(value.ToString());


            UpdateWorkersMoviePosition(value);
        }

        // set position background raw image 
        private void UpdateWorkersMoviePosition(uint value)
        {
            if (!maxWorkersValue.HasValue || !workersIcon.rectTransform)
            {
                return;
            }

            var position = workersIcon.rectTransform.localPosition;
            var offsetRate = endOffsetY - startOffsetY;
            var updatePositionY = startOffsetY + offsetRate * ((float)value / maxWorkersValue.Value);

            workersIcon.OnCancel();
            workersIcon.Animation(
                workersIcon.SlideTo(updatePositionY - position.y, AnimationAPI.SlideDirection.Vertical,
                    Easings.Default(1f)),
                new AnimationListener
                {
                    OnFinished = () =>
                    {
                        position.y = updatePositionY;
                        workersIcon.rectTransform.localPosition = position;
                    }
                }
            );
        }
    }
}