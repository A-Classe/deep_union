using Module.Task;
using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    ///     進捗率と明るさを同期するクラス
    /// </summary>
    public class ProgressToIntensity : MonoBehaviour
    {
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private AssignableAreaLight areaLight;

        [Header("進捗を0%とした時の明るさ")]
        [SerializeField]
        private float maxIntensity = 0.5f;

        private BaseTask task;

        private void Start()
        {
            task = transform.parent.GetComponent<BaseTask>();

            areaLight.SetIntensity(maxIntensity - maxIntensity * task.Progress);

            task.OnProgressChanged += progress =>
            {
                areaLight.SetIntensity(maxIntensity - maxIntensity * progress);

                if (progress >= 1f)
                {
                    assignableArea.enabled = false;
                }
            };
        }
    }
}