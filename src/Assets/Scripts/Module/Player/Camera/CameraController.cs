using System;
using Module.Player.Camera.State;
using UnityEngine;

namespace Module.Player.Camera
{
    public class CameraController : MonoBehaviour
    {
        private ICameraState currentState;
        private ICameraState[] states;

        private Transform followTarget;
        [SerializeField] private float depth = 14f;
        [SerializeField] private float followAngle = 85f;
        [SerializeField] private float cameraAngle = 65f;
        private void Awake()
        {
            Initialize();
        }

        private void FixedUpdate()
        {

            var angleInRadians = followAngle * Mathf.Deg2Rad;


            var y = depth * Math.Sin(angleInRadians);
            var horizontalDistance = depth * Math.Cos(angleInRadians);


            Vector3 horizontalOffset = -followTarget.forward * (float)horizontalDistance;


            Vector3 targetPosition = followTarget.position + horizontalOffset + Vector3.up * (float)y;

       
            transform.position = targetPosition;

         
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraAngle, 0f, 0f), Time.deltaTime);
        }

        private void Initialize()
        {
            states = new ICameraState[]
            {
                new FollowState(this),
                new IdleState()
            };
        }

        public void SetState(CameraState cameraState)
        {
            currentState = cameraState switch
            {
                CameraState.Follow => states[0],
                CameraState.Idle => states[1],
                _ => null
            };
        }

        public void InitParam()
        {
            
        }

        public void SetFollowTarget(Transform player)
        {
            followTarget = player;
        }

        public CameraState GetState() => currentState.GetState();
    }
}