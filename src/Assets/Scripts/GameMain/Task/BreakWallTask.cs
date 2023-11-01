using System;
using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace GameMain.Task
{
    /// <summary>
    /// - タスク終了後にgameObjectを削除する
    /// - 完了にしなければplayerは前に進めない
    /// </summary>
    public class BreakWallTask : BaseTask
    {
        /// <summary>
        /// タスク終了後にhitしたplayerのstateを変更する
        /// </summary>
        private Action onComplete;

        private RuntimeNavMeshBaker navMeshBaker;

        /// <summary>
        /// progressに応じて0,1.. lastのGameObject.activeを切り替える
        /// </summary>
        [SerializeField] private GameObject[] types;
        /// <summary>
        /// 表示中のobjectIndex
        /// </summary>
        private int currentIndex;
        

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            OnProgressChanged += OnProgress;

            if (types.Length < 1)
            {
                throw new NotImplementedException();
            }

            currentIndex = 0;
            UpdateObject();
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

        /// <summary>
        /// progressのcallbackを受け取る
        /// typesのlengthとcurrentIndexから次に切り替えが必要なタイミングを見つける
        /// </summary>
        /// <param name="value">progress count 0..1</param>
        private void OnProgress(float value)
        {
            float range =  (float)(currentIndex + 1) / (types.Length - 1);
            if (value >= range)
            {
                currentIndex++;
                UpdateObject();
            }
        }
        
        /// <summary>
        /// currentIndexに応じてobjectを切り替える
        /// </summary>
        private void UpdateObject()
        {
            foreach (var type in types)
            {
                type.SetActive(false);
            }
            types[currentIndex].SetActive(true);
        }

        public override void Disable() { }
    }
}