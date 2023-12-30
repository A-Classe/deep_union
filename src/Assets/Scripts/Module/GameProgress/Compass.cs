using UnityEngine;
using Wanna.DebugEx;

namespace Module.GameProgress
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] private Transform imageTransform;
        private Transform origin;
        private Transform target;
        private bool isEnable;

        public void SetOriginAndTarget(Transform origin, Transform target)
        {
            this.origin = origin;
            this.target = target;

            isEnable = true;
        }

        private void Update()
        {
            if (!isEnable)
            {
                return;
            }

            Vector3 targetDir = target.position - origin.position;
            Vector3 lookDir = origin.forward;

            float angle = Vector3.SignedAngle(targetDir, lookDir, Vector3.up);
            imageTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}