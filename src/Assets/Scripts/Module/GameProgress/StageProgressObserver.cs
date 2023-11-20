using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Player.Controller;
using UnityEngine;
using VContainer;

namespace System.GameProgress
{
    /// <summary>
    ///     ステージ進捗を監視するクラス
    /// </summary>
    public class StageProgressObserver
    {
        private readonly GoalPoint goalPoint;

        private readonly float initDistance;
        private readonly PlayerController playerController;
        private readonly CancellationTokenSource cTokenSource;

        [Inject]
        public StageProgressObserver(PlayerController playerController, GoalPoint goalPoint)
        {
            this.playerController = playerController;
            this.goalPoint = goalPoint;
            cTokenSource = new CancellationTokenSource();

            initDistance = GetDistance();
        }

        public float Progress => Mathf.InverseLerp(initDistance, 0f, GetDistance());

        public event Action OnCompleted;

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

        public float GetDistance()
        {
            return (goalPoint.transform.position - playerController.transform.position).z;
        }
    }
}