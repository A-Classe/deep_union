using System;
using Module.Player.Camera.State;
using UnityEngine;

namespace Module.Player.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : MonoBehaviour
    {
        private ICameraState currentState;
        private ICameraState[] states;

        private Transform followTarget;
        private float depth = 14f;
        private double followAngle = 85f;
        private double cameraAngle = 65f;
        private void Awake()
        {
            Initialize();
            
        }

        private void FixedUpdate()
        {
            if (followTarget == null) return;
            var angleInRadians = followAngle * Mathf.Deg2Rad;


            var y = depth * Math.Sin(angleInRadians);
            var horizontalDistance = depth * Math.Cos(angleInRadians);


            Vector3 horizontalOffset = -followTarget.forward * (float)horizontalDistance;


            Vector3 targetPosition = followTarget.position + horizontalOffset + Vector3.up * (float)y;

       
            transform.position = targetPosition;

         
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((float)cameraAngle, 0f, 0f), Time.deltaTime);
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
            
            var position = transform.position;
            depth = Vector3.Distance(position, player.position);
            
            var angle = Math.Atan2(position.y - player.position.y, position.z - player.position.z);
            followAngle = 180d - (angle * 180d / Math.PI);

            cameraAngle = transform.rotation.eulerAngles.x;
        }

        public CameraState GetState() => currentState.GetState();

        public UnityEngine.Camera GetCamera()
        {
            return GetComponent<UnityEngine.Camera>();
        }
    }
}