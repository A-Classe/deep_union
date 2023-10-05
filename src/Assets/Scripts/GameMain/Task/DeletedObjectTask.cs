using System;
using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    /// <summary>
    /// - タスク終了後にgameObjectを削除する
    /// - 完了にしなければplayerは前に進めない
    /// </summary>
    public class DeletedObjectTask : BaseTask
    {
        /// <summary>
        /// タスク終了後にhitしたplayerのstateを変更する
        /// </summary>
        private Action onComplete;

        private RuntimeNavMeshBaker navMeshBaker;

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
        }

        protected override void OnComplete()
        {
            DisableSequence().Forget();
        }

        private async UniTaskVoid DisableSequence()
        {
            SetDetection(false);
            Disable();

            await navMeshBaker.Bake();

            onComplete?.Invoke();
        }

        /// <summary>
        /// 子objectのwaitPointからタスク完了までplayerをその場に止める
        /// </summary>
        /// <param name="other">hitしたgameObject</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            var obj = other.gameObject;

            var controller = obj.GetComponent<PlayerController>();
            if (controller == null) return;

            controller.SetState(PlayerState.Wait);
            onComplete = () => controller.SetState(PlayerState.Go);
        }
    }
}