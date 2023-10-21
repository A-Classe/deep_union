using System.Collections.Generic;
using DG.Tweening;
using Module.Assignment.Component;
using Module.Task;
using Module.Working;
using Module.Working.Factory;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    public class IncreaseWorkerTask : BaseTask
    {
        [Header("増やすワーカーのリスト")]
        [SerializeField]
        private List<Worker> imprisonedWorkers;


        [SerializeField] private float maxCutOfHeight = 12;
        [SerializeField] private float duration = 2;
        [SerializeField] private Renderer sphereRenderer;
        [SerializeField] private ParticleSystem floorParticle;
        private Material waveMaterial;
        private Material fresnelMaterial;
        private static readonly int CutOfHeightKey = Shader.PropertyToID("_CutOfHeight");

        private WorkerAgent workerAgent;
        private LeaderAssignableArea leaderAssignableArea;
        private SpawnPoint spawnPoint;

        public override void Initialize(IObjectResolver container)
        {
            workerAgent = container.Resolve<WorkerAgent>();
            leaderAssignableArea = container.Resolve<LeaderAssignableArea>();
            spawnPoint = container.Resolve<SpawnPoint>();

            foreach (Worker worker in imprisonedWorkers)
            {
                worker.Initialize().Forget();
                worker.SetLockState(true);
            }

            Material[] materials = sphereRenderer.materials;
            waveMaterial = materials[0];
            fresnelMaterial = materials[1];
        }

        protected override void OnComplete()
        {
            foreach (Worker worker in imprisonedWorkers)
            {
                if (!leaderAssignableArea.AssignableArea.CanAssign())
                    return;

                worker.SetLockState(false);
                workerAgent.AddActiveWorker(worker);

                leaderAssignableArea.AssignableArea.AddWorker(worker);
                worker.transform.SetParent(spawnPoint.transform);
            }

            waveMaterial.DOFloat(maxCutOfHeight, CutOfHeightKey, duration).Play();
            fresnelMaterial.DOFloat(maxCutOfHeight, CutOfHeightKey, duration).Play();
            floorParticle.Stop();
        }
    }
}