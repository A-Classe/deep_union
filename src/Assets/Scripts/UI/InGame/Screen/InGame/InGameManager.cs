using Core.Utility.UI.Navigation;
using TMPro;
using Unity.Plastic.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame.Screen.InGame
{
    public class InGameManager: UIManager
    {
        // hp
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI maxHp;
        [SerializeField] private Slider sliderHp;
        [SerializeField] private Image gaugeHpBackground;
        private uint? maxHpValue;
        
        // progress
        [SerializeField] private TextMeshProUGUI currentProgress;
        
        // resource
        [SerializeField] private TextMeshProUGUI currentRes;
        [SerializeField] private TextMeshProUGUI maxRes;
        [SerializeField] private Image gaugeRes;
        private uint? maxResValue;

        private const float StartOffsetAmount = 0.37f;
        private const float EndOffsetAmount = 0.63f;

        // workers
        [SerializeField] private TextMeshProUGUI currentWorkers;
        [SerializeField] private RectTransform workersIcon;
        private uint? maxWorkersValue;
        [SerializeField] private float startOffsetY;// -205
        [SerializeField] private float endOffsetY; // -37

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
                throw new MismatchedNotSetException();
            }

            if (current > maxHpValue.Value) return;

            currentHp.text = current.ToString();
            sliderHp.value = current;
            gaugeHpBackground.color = current switch {
                _ when current > maxHpValue.Value * 0.3f => Color.green,
                _ => Color.red
            };
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
                maxRes.text = max.Value.ToString();
            }

            if (!maxResValue.HasValue)
            {
                throw new MismatchedNotSetException();
            }

            if (current > maxResValue.Value) return;

            currentRes.text = current.ToString();
            float rate = (float)current / maxResValue.Value;
            gaugeRes.fillAmount = StartOffsetAmount + (EndOffsetAmount - StartOffsetAmount) * rate;
        }

        public void SetWorkerCount(uint value, uint? max = null)
        {
            if (max.HasValue)
            {
                maxWorkersValue = max.Value;
            }
            
            if (!maxWorkersValue.HasValue)
            {
                throw new MismatchedNotSetException();
            }
            
            if (value > maxWorkersValue.Value) return;

            currentWorkers.text = value.ToString();
            
            // set position background raw image 
            Vector3 position = workersIcon.localPosition;
            float offsetRate = endOffsetY - startOffsetY;
            position.y = startOffsetY + offsetRate * ((float)value / maxWorkersValue.Value);
            
            workersIcon.localPosition = position;
        }
    }
}