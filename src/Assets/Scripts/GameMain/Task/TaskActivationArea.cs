using System;
using Module.Task;
using UnityEngine;
using Wanna.DebugEx;

namespace GameMain.Task
{
    public class TaskActivationArea : MonoBehaviour
    {
        private BaseTask[] baseTasks;

        private void Start()
        {
            baseTasks = GetComponentsInChildren<BaseTask>(true);
        }

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