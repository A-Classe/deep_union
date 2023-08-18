using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Module.Worker
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;
        private Vector3 offset;

        public void OnSpawn(Vector3 spawnPoint)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            offset = transform.position - spawnPoint;
        }

        public void SetFollowPoint(Vector3 point)
        {
            navMeshAgent.SetDestination(point + offset);
        }
    }
}