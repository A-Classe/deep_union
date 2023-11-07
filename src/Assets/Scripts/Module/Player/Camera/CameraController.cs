using System;
using Module.Player.Camera.State;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Player.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform leaderTarget;
        [SerializeField] private float maxRotationRate = 0.3f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float upRate = 1f;
        [SerializeField] private float inclination = 1f;
        [SerializeField] private float vertical = 1f;
        [SerializeField] private float minAngleX = 40f;
        [SerializeField] private float maxAngleX = 50f;

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

        private void LateUpdate()
        {
            if (followTarget == null) return;
            var angleInRadians = followAngle * Mathf.Deg2Rad;

            var y = depth * Math.Sin(angleInRadians);
            var horizontalDistance = depth * Math.Cos(angleInRadians);
            //var distance = Vector3.Distance(followTarget.position, leaderTarget.position);
            var distance = leaderTarget.position.z - followTarget.position.z;

            Vector3 horizontalOffset = -followTarget.forward * (float)horizontalDistance;
            Vector3 targetPosition = followTarget.position + horizontalOffset + Vector3.up * (float)y;

            targetPosition.y = (-Mathf.Atan(inclination * (distance - vertical / inclination)) + Mathf.PI / 2) * upRate + targetPosition.y;

            transform.position = targetPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((float)cameraAngle, 0f, 0f), Time.deltaTime);

            RotateToTarget();
        }

        private void RotateToTarget()
        {
            var targetRotation = Quaternion.LookRotation(leaderTarget.position - transform.position);
            var slerpRotation = Quaternion.Slerp(transform.rotation, targetRotation, maxRotationRate);

            Vector3 eulerAngles = slerpRotation.eulerAngles;
            eulerAngles.x = Mathf.Clamp(eulerAngles.x, minAngleX, maxAngleX);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(eulerAngles), Time.fixedDeltaTime * rotationSpeed);
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

        public void InitParam() { }

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