using UnityEngine;

namespace Module.Extension.Task
{
    public class EnemyActivationArea : MonoBehaviour
    {
        private Enemy1Task[] baseTasks;

        private void Start()
        {
            baseTasks = GetComponentsInChildren<Enemy1Task>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                foreach (var enemy1Task in baseTasks)
                {
                    enemy1Task.ForceEnable();
                    enemy1Task.SetDetection(true);
                }
        }
    }
}