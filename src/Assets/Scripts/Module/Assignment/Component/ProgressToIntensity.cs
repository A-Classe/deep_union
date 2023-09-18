using Module.Task;
using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    /// 進捗率と明るさを同期するクラス
    /// </summary>
    public class ProgressToIntensity : MonoBehaviour
    {
        [SerializeField] private BaseTask task;
        [SerializeField] private AssignableArea assignableArea;

        [Header("進捗を0%とした時の明るさ")]
        [SerializeField]
        private float maxIntensity = 0.5f;

        private void Start()
        {
            assignableArea.SetLightIntensity(maxIntensity - maxIntensity * task.Progress);

            task.OnProgressChanged += progress =>
            {
                assignableArea.SetLightIntensity(maxIntensity - maxIntensity * progress);
            };
        }
    }
}