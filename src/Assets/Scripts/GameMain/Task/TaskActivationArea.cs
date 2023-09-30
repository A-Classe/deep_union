using System;
using Module.Task;
using UnityEngine;

namespace GameMain.Task
{
    public class TaskActivationArea : MonoBehaviour
    {
        [SerializeField] private BaseTask[] baseTasks;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (BaseTask baseTask in baseTasks)
                {
                    baseTask.Enable();
                    baseTask.SetDetection(true);
                }
            }
        }
    }
}