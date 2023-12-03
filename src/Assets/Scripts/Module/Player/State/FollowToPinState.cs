using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Player.State
{
    /// <summary>
    /// 潜水艦がピンを追いかけるステート
    /// </summary>
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
            //Rigidbodyの無効化
            rig.velocity = Vector3.zero;
            rig.isKinematic = true;
            
            //NavMeshAgentの初期化
            navMeshAgent.enabled = true;

            //探索可能になるまで待機
            await UniTask.WaitWhile(() => navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid);
            
            navMeshAgent.ResetPath();
        }

        public void Stop()
        {
            navMeshAgent.enabled = false;
            rig.isKinematic = false;
        }

        public void FixedUpdate()
        {
            //探索可能になるまで待機
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return;
            
            navMeshAgent.SetDestination(followPin.GetPosition());

            //探索完了するまで移動しない
            if (navMeshAgent.pathPending)
                return;

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                followPin.ArriveToPin();
            }
        }
    }
}