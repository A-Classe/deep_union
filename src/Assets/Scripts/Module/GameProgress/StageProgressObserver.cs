using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using UnityEngine;
using VContainer;
using Wanna.DebugEx;

namespace System.GameProgress
{
    /// <summary>
    /// ステージ進捗を監視するクラス
    /// </summary>
    public class StageProgressObserver
    {
        private readonly PlayerController playerController;
        private readonly GoalPoint goalPoint;
        private readonly CancellationTokenSource cTokenSource;

        public event Action OnCompleted;

        public float Progress => Mathf.InverseLerp(initDistance, 0f, GetDistance());

        private readonly float initDistance;

        [Inject]
        public StageProgressObserver(PlayerController playerController, GoalPoint goalPoint)
        {
            this.playerController = playerController;
            this.goalPoint = goalPoint;
            cTokenSource = new CancellationTokenSource();

            initDistance = GetDistance();
        }

        public async UniTaskVoid Start()
        {
            //ゴール到着まで待機
            await UniTask.WaitUntil(() => Progress >= 1f, cancellationToken: cTokenSource.Token);

            if (cTokenSource.IsCancellationRequested)
                return;

            OnCompleted?.Invoke();
        }

        public void Cancel()
        {
            cTokenSource.Cancel();
            cTokenSource.Dispose();
        }

        float GetDistance()
        {
            // todo: シーンを切り替え直後にnullチェック行う
            if (goalPoint == null)
            {
                return 0f;
            }
            return (goalPoint.transform.position - playerController.transform.position).z;
        }
    }
}