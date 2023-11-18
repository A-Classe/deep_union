using System;
using UnityEngine;

namespace Module.Minimap
{
    public class MinimapController: MonoBehaviour
    {
        [SerializeField] private Transform playerFocus;

        private float focusDepth = 50f;

        private Vector3 lastPosition = Vector3.zero;

        private void Awake()
        {
            if (playerFocus == null)
            {
                throw new NotImplementedException();
            }
        }

        private void Update()
        {
            
            if (IfNeedUpdate(playerFocus.position, lastPosition)) return;
            
            var playerPos = playerFocus.position;
            playerPos.y += focusDepth;
            transform.position = playerPos;
            lastPosition = playerPos;
            transform.LookAt(playerFocus);
            var camPos = transform.position;
            camPos += playerFocus.forward * 20f;
            transform.position = camPos;
        }

        private bool IfNeedUpdate(Vector3 a, Vector3 b)
        {
            Vector3 aPos = a;
            aPos.y = 0f;
            Vector3 bPos = b;
            bPos.y = 0f;
            return Vector3.Distance(aPos, bPos) < 1f;
        }
    }
}