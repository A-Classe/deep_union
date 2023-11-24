using Core.Input;
using UnityEngine;

namespace Module.Player.Camera
{
    public class VirtualCameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float speed;

        private InputEvent rotateEvent;

        private void Start()
        {
            rotateEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.RotateCamera);
        }

        private void Update()
        {
            float delta = rotateEvent.ReadValue<float>() * speed * Time.deltaTime;
            followTarget.Rotate(0f, delta, 0f);
        }
    }
}