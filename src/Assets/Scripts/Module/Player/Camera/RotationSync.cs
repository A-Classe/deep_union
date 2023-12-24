using UnityEngine;

namespace Module.Player.Camera
{
    public class RotationSync : MonoBehaviour
    {
        [SerializeField] private Transform origin;

        private void Update()
        {
            transform.rotation = Quaternion.Euler(0, origin.eulerAngles.y, 0);
        }
    }
}