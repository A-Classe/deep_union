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
        [SerializeField] private bool enableAgentOnStart;
        private NavMeshAgent navMeshAgent;
        private Vector3 offset;

        public void OnSpawn(Vector3 spawnPoint)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = enableAgentOnStart;

            offset = transform.position - spawnPoint;
        }

        public void SetFollowPoint(Vector3 point)
        {
            if (enableAgentOnStart)
            {
                navMeshAgent.SetDestination(point);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, point + offset, Time.deltaTime * 2f);
            }
        }
    }
}