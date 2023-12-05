using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class ValidButton: AnimationBehaviour
    {
        [NonSerialized] public RectTransform rectTransform;

        [SerializeField] private GameObject grey;

        public bool IsValid => grey.activeSelf;
        
        private float minRotationSpeed = 130.0f;
        private float maxRotationSpeed = 240.0f;
        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            SetValidVisual(false);
        }
        private void Update()
        {
            if (grey.activeSelf)
            {
                float currentAngle = grey.transform.eulerAngles.z;
                currentAngle = (currentAngle > 180) ? currentAngle - 360 : currentAngle;
                
                float normalizedSpeed = (Mathf.Sin(currentAngle * Mathf.Deg2Rad) + 1) / 2;
                
                float rotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, normalizedSpeed);
                
                transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            }
        }

        public void SetValidVisual(bool isActive)
        {
            grey.SetActive(isActive);
            transform.rotation = Quaternion.identity;
        }
    }
}