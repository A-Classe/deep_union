using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.HUD
{
    public class TaskProgressView : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;

        private Camera cam;
        private Transform target;

        private void Awake()
        {
            cam = Camera.main;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void ManagedUpdate()
        {
            float xSign = Mathf.Sign(target.position.x - cam.transform.position.x);
            Vector3 screenPoint = cam.WorldToScreenPoint(target.position + new Vector3(xSign * offset.x, offset.y, offset.z));
            transform.position = screenPoint;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}