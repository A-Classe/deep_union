using System;
using UnityEngine;

namespace Module.Extension.Task
{
    public class ResourceItem : MonoBehaviour
    {
        private Action onCollide;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                onCollide?.Invoke();
                Destroy(gameObject);
            }
        }

        public void SetCollideEvent(Action onCollide)
        {
            this.onCollide = onCollide;
        }
    }
}