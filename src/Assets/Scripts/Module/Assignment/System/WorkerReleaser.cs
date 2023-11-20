using System.Collections.Generic;
using System.Linq;
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

        private IReadOnlyList<AssignableArea> activeAreas;

        [Inject]
        public WorkerReleaser(LeaderAssignableArea leaderAssignableArea)
        {
            this.leaderAssignableArea = leaderAssignableArea;

            releaseWorkerCache = new Worker[64];
        }

        public void SetActiveAreas(IReadOnlyList<AssignableArea> activeAreas)
        {
            this.activeAreas = activeAreas;
        }

        public void Update()
        {
            var leaderEllipse = leaderAssignableArea.AssignableArea.EllipseData;
            var leaderArea = leaderAssignableArea.AssignableArea;

            foreach (var activeArea in activeAreas)
            {
                //範囲同士の円形判定を行い、計算が必要な範囲を絞る
                if (!CollisionUtil.IsCollideCircle(leaderEllipse, activeArea.EllipseData))
                    continue;

                var cacheCount = 0;

                foreach (var worker in activeArea.AssignedWorkers)
                    if (CollisionUtil.InEllipse(worker.transform.position, leaderArea.EllipseData))
                    {
                        //イテレート中はコレクション変更できないので一旦キャッシュ
                        releaseWorkerCache[cacheCount] = worker;
                        cacheCount++;
                    }

                for (var i = 0; i < cacheCount; i++)
                {
                    var worker = releaseWorkerCache[i];

                    if (leaderArea.CanAssign())
                    {
                        activeArea.RemoveWorker(worker);
                        leaderArea.AddWorker(worker);
                    }
                }
            }
        }

        public void ReleaseAllWorkers(AssignableArea assignableArea)
        {
            var workers = assignableArea.AssignedWorkers.ToArray();

            foreach (var worker in workers)
                if (leaderAssignableArea.AssignableArea.CanAssign())
                {
                    assignableArea.RemoveWorker(worker);
                    leaderAssignableArea.AssignableArea.AddWorker(worker);
                }
        }
    }
}