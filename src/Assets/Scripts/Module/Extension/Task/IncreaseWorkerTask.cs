using System.Collections.Generic;
using DG.Tweening;
using Module.Assignment.Component;
using Module.Task;
using Module.Working;
using Module.Working.Factory;
using UnityEngine;
using VContainer;

namespace Module.Extension.Task
{
    public class IncreaseWorkerTask : BaseTask
    {
        private static readonly int CutOfHeightKey = Shader.PropertyToID("_CutOfHeight");

        [Header("増やすワーカーのリスト")] [SerializeField]
        private List<Worker> imprisonedWorkers;


        [SerializeField] private float maxCutOfHeight = 12;
        [SerializeField] private float duration = 2;
        [SerializeField] private Renderer sphereRenderer;
        [SerializeField] private ParticleSystem floorParticle;
        private LeaderAssignableArea leaderAssignableArea;
        private SpawnPoint spawnPoint;
        private Material waveMaterial;

        private WorkerAgent workerAgent;

        public override void Initialize(IObjectResolver container)
        {
            workerAgent = container.Resolve<WorkerAgent>();
            leaderAssignableArea = container.Resolve<LeaderAssignableArea>();
            spawnPoint = container.Resolve<SpawnPoint>();

            foreach (var worker in imprisonedWorkers)
            {
                worker.Initialize().Forget();
                worker.SetLockState(true);
            }

            var materials = sphereRenderer.materials;
            waveMaterial = materials[0];
        }

        protected override void OnComplete()
        {
            foreach (var worker in imprisonedWorkers)
            {
                if (!leaderAssignableArea.AssignableArea.CanAssign())
                    return;

                worker.SetLockState(false);
                workerAgent.AddActiveWorker(worker);

                leaderAssignableArea.AssignableArea.AddWorker(worker, AssignableArea.WorkerEventType.Create);
                worker.transform.SetParent(spawnPoint.transform);
            }

            waveMaterial.DOFloat(maxCutOfHeight, CutOfHeightKey, duration).Play();
            floorParticle.Stop();
        }
    }
}