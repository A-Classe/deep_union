using UnityEngine;

namespace Debug
{
    public class DestroyOnStart:MonoBehaviour
    {
        [SerializeField] private bool destroyOnStart;
        [SerializeField] private GameObject graphy;

        private void Start()
        {
            if (destroyOnStart)
            {
                Destroy(gameObject);
                Destroy(graphy);
            }
        }
    }
}