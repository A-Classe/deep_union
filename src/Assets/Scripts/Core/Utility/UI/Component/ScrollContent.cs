using System;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class ScrollContent: MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        private const float ScrollDuration = 10f;
        private float startY;
        private float endY;
        private float timer;

        public event Action OnScrollFinished;

        private bool isEnable;
        
        public bool IsEnable => isEnable;
        
        public void Awake()
        {
            startY = content.anchoredPosition.y;
            endY = startY + content.rect.height - GetComponent<RectTransform>().rect.height;
        }

        public void Play()
        {
            timer = 0f;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, startY);
            isEnable = true;
        }

        private void FixedUpdate()
        {
            if (!isEnable) return;
            if (timer < ScrollDuration)
            {
                timer += Time.deltaTime;
                float newY = Mathf.Lerp(startY, endY, timer / ScrollDuration);
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, newY);
            }
            else
            {
                isEnable = false;
                OnScrollFinished?.Invoke();
            }
        }
    }
}