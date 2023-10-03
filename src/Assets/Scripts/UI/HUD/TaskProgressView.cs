using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    /// <summary>
    /// タスクの進捗バーコンポーネント
    /// </summary>
    public class TaskProgressView : MonoBehaviour
    {
        [Header("表示オフセット")] [SerializeField] private Vector3 offset;
        [SerializeField] private float scaleOffset;
        [SerializeField] private Slider slider;

        private Camera cam;
        private Transform target;

        public bool IsEnabled => gameObject.activeSelf;

        private void Awake()
        {
            cam = Camera.main;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void ManagedUpdate()
        {
            var targetPos = target.position;
            var camPos = cam.transform.position;

            float xSign = Mathf.Sign(targetPos.x - camPos.x);
            Vector3 screenPoint = cam.WorldToScreenPoint(targetPos + new Vector3(xSign * offset.x, offset.y, offset.z));
            transform.position = screenPoint;

            float distance = Vector3.Distance(targetPos, camPos);
            transform.localScale = (Vector3.one / distance) * scaleOffset;
        }

        /// <summary>
        /// 進捗をセットします(0 ~ 1)
        /// </summary>
        /// <param name="value">進捗</param>
        public void SetProgress(float value)
        {
            slider.value = value;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}