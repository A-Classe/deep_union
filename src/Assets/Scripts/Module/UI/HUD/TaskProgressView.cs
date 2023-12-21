using UnityEngine;
using UnityEngine.UI;

namespace Module.UI.HUD
{
    /// <summary>
    ///     タスクの進捗バーコンポーネント
    /// </summary>
    public class TaskProgressView : MonoBehaviour
    {
        [Header("表示オフセット")] [SerializeField] private Vector3 offset;
        [SerializeField] private float scaleOffset;
        [SerializeField] private Slider slider;

        private Camera cam;
        private Transform target;
        private static readonly Vector3 OutSideCamera = new(1000000f, 1000000f, 0f);

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

            //カメラ上のスクリーン座標を取得
            var xSign = Mathf.Sign(targetPos.x - camPos.x);
            var screenPoint = cam.WorldToScreenPoint(targetPos + new Vector3(xSign * offset.x, offset.y, offset.z));

            //カメラの後ろにいるときは遠くに飛ばす
            if (screenPoint.z <= 0f)
            {
                transform.position = OutSideCamera;
                return;
            }

            transform.position = screenPoint;

            //距離に応じてスケール
            var distance = Vector3.Distance(targetPos, camPos);
            transform.localScale = Vector3.one / distance * scaleOffset;
        }

        /// <summary>
        ///     進捗をセットします(0 ~ 1)
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