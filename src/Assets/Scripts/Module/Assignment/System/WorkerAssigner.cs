using System.Collections.Generic;
using Core.Model.User;
using Core.User.Recorder;
using Module.Assignment.Component;
using Module.Assignment.Utility;
using Module.Working;
using Unity.Burst;
using UnityEngine;
using VContainer;
using Wanna.DebugEx;

namespace Module.Assignment.System
{
    [BurstCompile]
    public class WorkerAssigner
    {
        private readonly Worker[] assignWorkerCache;
        private readonly LeaderAssignableArea leaderAssignableArea;

        private readonly EventBroker eventBroker;

        private IReadOnlyList<AssignableArea> activeAreas;

        [Inject]
        public WorkerAssigner(
            LeaderAssignableArea leaderAssignableArea,
            EventBroker eventBroker
        )
        {
            this.leaderAssignableArea = leaderAssignableArea;

            this.eventBroker = eventBroker;

            assignWorkerCache = new Worker[128];
        }

        public void SetActiveAreas(IReadOnlyList<AssignableArea> activeAreas)
        {
            this.activeAreas = activeAreas;
        }

        public void Update()
        {
            var leaderArea = leaderAssignableArea.AssignableArea;
            var leaderWorkers = leaderAssignableArea.AssignableArea.AssignedWorkers;


            foreach (var activeArea in activeAreas)
            {
                //範囲同士の円形判定を行い、計算が必要な範囲を絞る
                if (!activeArea.enabled ||
                    !CollisionUtil.IsCollideCircle(leaderArea.EllipseData, activeArea.EllipseData))
                    continue;

                var cacheCount = 0;

                foreach (var worker in leaderWorkers)
                    if (CollisionUtil.InEllipse(worker.transform.position, activeArea.EllipseData))
                    {
                        if (cacheCount >= assignWorkerCache.Length)
                        {
                            DebugEx.LogError("リーダーの割り当て上限を超えました！");
                            return;
                        }

                        assignWorkerCache[cacheCount] = worker;
                        cacheCount++;
                    }

                for (var i = 0; i < cacheCount; i++)
                {
                    var worker = assignWorkerCache[i];

                    if (activeArea.CanAssign())
                    {
                        leaderAssignableArea.AssignableArea.RemoveWorker(worker);
                        activeArea.AddWorker(worker);
                        eventBroker.SendEvent(new AssignEvent().Event());
                    }
                }
            }
        }
    }
}