using System;
using UnityEngine;

namespace GameMain.Task
{
    public class ResourceItem : MonoBehaviour
    {
        private Action onCollide;

        public void SetCollideEvent(Action onCollide)
        {
            this.onCollide = onCollide;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                onCollide?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}