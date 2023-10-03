using System.Collections.Generic;
using Module.Assignment.Component;
using Module.Assignment.Utility;
using Module.Working;
using Unity.Burst;
using VContainer;
using Wanna.DebugEx;

namespace Module.Assignment.System
{
    [BurstCompile]
    public class WorkerAssigner
    {
        private readonly LeaderAssignableArea leaderAssignableArea;
        private readonly Worker[] assignWorkerCache;

        private IReadOnlyList<AssignableArea> activeAreas;

        [Inject]
        public WorkerAssigner(LeaderAssignableArea leaderAssignableArea)
        {
            this.leaderAssignableArea = leaderAssignableArea;

            assignWorkerCache = new Worker[64];
        }

        public void SetActiveAreas(IReadOnlyList<AssignableArea> activeAreas)
        {
            this.activeAreas = activeAreas;
        }

        public void Update()
        {
            AssignableArea leaderArea = leaderAssignableArea.AssignableArea;
            IReadOnlyList<Worker> leaderWorkers = leaderAssignableArea.AssignableArea.AssignedWorkers;

            foreach (AssignableArea activeArea in activeAreas)
            {
                //範囲同士の円形判定を行い、計算が必要な範囲を絞る
                if (!activeArea.enabled || !CollisionUtil.IsCollideCircle(leaderArea.EllipseData, activeArea.EllipseData))
                    continue;

                int cacheCount = 0;

                foreach (Worker worker in leaderWorkers)
                {
                    if (CollisionUtil.InEllipse(worker.transform.position, activeArea.EllipseData))
                    {
                        assignWorkerCache[cacheCount] = worker;
                        cacheCount++;
                    }
                }

                for (int i = 0; i < cacheCount; i++)
                {
                    Worker worker = assignWorkerCache[i];

                    if (activeArea.CanAssign())
                    {
                        leaderAssignableArea.AssignableArea.RemoveWorker(worker);
                        activeArea.AddWorker(worker);
                    }
                }
            }
        }
    }
}