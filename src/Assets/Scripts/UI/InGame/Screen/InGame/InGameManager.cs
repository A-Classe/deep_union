using Core.Utility.UI.Navigation;
using TMPro;
using Unity.Plastic.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.InGame.Screen.InGame
{
    // todo: 
    public class InGameManager: UIManager
    {
        // hp
        [SerializeField] private TextMeshProUGUI currentHp;
        [SerializeField] private TextMeshProUGUI maxHp;
        [SerializeField] private Slider sliderHp;
        private uint? maxHpValue;
        
        // progress
        [SerializeField] private TextMeshProUGUI currentProgress;
        
        // resource
        [SerializeField] private TextMeshProUGUI currentRes;
        [SerializeField] private TextMeshProUGUI maxRes;
        [SerializeField] private Image gaugeRes;
        private uint? maxResValue;
        
        // workers
        [SerializeField] private TextMeshProUGUI currentWorkers;

        public override void Select(Vector2 direction)
        {
            // NOP.
        }

        public override void Clicked()
        {
            // NOP.
        }

        public void SetHP(uint current, uint? max = null)
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
            gaugeRes.fillAmount = (float)current / maxResValue.Value;
        }

        public void SetWorkerCount(uint value)
        {
            currentWorkers.text = value.ToString();
        }
        
    }
}