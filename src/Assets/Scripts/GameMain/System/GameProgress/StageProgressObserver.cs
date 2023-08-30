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

        [Inject]
        public StageProgressObserver(PlayerController playerController, GoalPoint goalPoint)
        {
            this.playerController = playerController;
            this.goalPoint = goalPoint;
            cTokenSource = new CancellationTokenSource();
        }

        public async UniTaskVoid Start()
        {
            //ゴール到着まで待機
            await WaitForArrived(cTokenSource.Token);

            if (cTokenSource.IsCancellationRequested)
                return;

            OnCompleted?.Invoke();
        }

        public void Cancel()
        {
            cTokenSource.Cancel();
            cTokenSource.Dispose();
        }

        UniTask WaitForArrived(CancellationToken cToken)
        {
            return UniTask.WaitUntil(IsArrived, cancellationToken: cToken);

            bool IsArrived()
            {
                Vector3 goalPos = goalPoint.transform.position;
                Vector3 playerPos = playerController.transform.position;

                return (goalPos - playerPos).z <= 0;
            }
        }
    }
}