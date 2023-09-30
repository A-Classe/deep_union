using Module.Task;
using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    /// 進捗率と明るさを同期するクラス
    /// </summary>
    public class ProgressToIntensity : MonoBehaviour
    {
        [SerializeField] private AssignableArea assignableArea;

        private BaseTask task;

        [Header("進捗を0%とした時の明るさ")]
        [SerializeField]
        private float maxIntensity = 0.5f;

        private void Start()
        {
            task = transform.parent.GetComponent<BaseTask>();

            assignableArea.SetLightIntensity(maxIntensity - maxIntensity * task.Progress);

            task.OnProgressChanged += progress =>
            {
                assignableArea.SetLightIntensity(maxIntensity - maxIntensity * progress);

                if (progress >= 1f)
                {
                    assignableArea.enabled = false;
                }
            };
        }
    }
}