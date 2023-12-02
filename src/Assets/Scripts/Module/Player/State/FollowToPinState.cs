using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Player.State
{
    internal class FollowToPinState : IPlayerState
    {
        private readonly Rigidbody rig;
        private readonly NavMeshAgent navMeshAgent;
        private readonly FollowPin followPin;

        public FollowToPinState(Rigidbody rig, NavMeshAgent navMeshAgent, FollowPin followPin)
        {
            this.rig = rig;
            this.navMeshAgent = navMeshAgent;
            this.followPin = followPin;
        }

        public PlayerState GetState()
        {
            return PlayerState.FollowToPin;
        }

        public async void Start()
        {
            rig.velocity = Vector3.zero;
            navMeshAgent.enabled = true;

            await UniTask.WaitWhile(() => navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid);
            
            navMeshAgent.ResetPath();
        }

        public void Stop()
        {
            navMeshAgent.enabled = false;
        }

        public void FixedUpdate()
        {
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return;

            navMeshAgent.SetDestination(followPin.GetPosition());

            if (navMeshAgent.pathPending)
                return;

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                DebugEx.Log("Arrived");
                followPin.ArriveToPin();
            }
        }
    }
}