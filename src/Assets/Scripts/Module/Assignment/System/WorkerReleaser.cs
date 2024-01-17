using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model.User;
using Core.User.Recorder;
using Module.Assignment.Component;
using Module.Assignment.Utility;
using Module.Working;
using VContainer;

namespace Module.Assignment.System
{
    public class WorkerReleaser
    {
        private readonly LeaderAssignableArea leaderAssignableArea;
        private readonly Worker[] releaseWorkerCache;

        private readonly EventBroker eventBroker;

        private IReadOnlyList<AssignableArea> activeAreas;
        public event Action OnRelease;

        [Inject]
        public WorkerReleaser(
            LeaderAssignableArea leaderAssignableArea,
            EventBroker eventBroker
        )
        {
            this.leaderAssignableArea = leaderAssignableArea;

            this.eventBroker = eventBroker;

            releaseWorkerCache = new Worker[64];
        }

        public void SetActiveAreas(IReadOnlyList<AssignableArea> activeAreas)
        {
            this.activeAreas = activeAreas;
        }

        public void Update()
        {
            var leaderEllipse = leaderAssignableArea.AssignableArea.AreaShape;
            var leaderArea = leaderAssignableArea.AssignableArea;

            foreach (var activeArea in activeAreas)
            {
                //範囲同士の円形判定を行い、計算が必要な範囲を絞る
                if (!CollisionUtil.IsCollideCircle(leaderEllipse, activeArea.AreaShape))
                {
                    continue;
                }

                var cacheCount = 0;

                foreach (var worker in activeArea.AssignedWorkers)
                {
                    if (CollisionUtil.InEllipse(worker.transform.position, leaderArea.AreaShape))
                    {
                        //イテレート中はコレクション変更できないので一旦キャッシュ
                        releaseWorkerCache[cacheCount] = worker;
                        cacheCount++;
                    }
                }

                for (var i = 0; i < cacheCount; i++)
                {
                    var worker = releaseWorkerCache[i];

                    if (leaderArea.CanAssign())
                    {
                        activeArea.RemoveWorker(worker);
                        leaderArea.AddWorker(worker);
                        eventBroker.SendEvent(new ReleaseEvent().Event());
                        OnRelease?.Invoke();
                    }
                }
            }
        }

        public void ReleaseAllWorkers(AssignableArea assignableArea)
        {
            var workers = assignableArea.AssignedWorkers.ToArray();

            foreach (var worker in workers)
            {
                if (leaderAssignableArea.AssignableArea.CanAssign())
                {
                    assignableArea.RemoveWorker(worker);
                    leaderAssignableArea.AssignableArea.AddWorker(worker);
                    OnRelease?.Invoke();
                }
            }
        }
    }
}