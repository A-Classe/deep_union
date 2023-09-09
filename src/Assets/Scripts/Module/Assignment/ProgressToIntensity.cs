using System;
using System.Collections;
using System.Collections.Generic;
using Module.Task;
using UnityEngine;

namespace Module.Assignment
{
    public class ProgressToIntensity : MonoBehaviour
    {
        [SerializeField] private BaseTask task;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private float maxIntensity = 0.5f;

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