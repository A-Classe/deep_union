using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Worker
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TaskWorker : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetFollowPoint(Vector3 point)
        {
            navMeshAgent.SetDestination(point);
        }
    }
}