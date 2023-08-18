using UnityEngine;

namespace Module.Worker.Factory
{
    public readonly struct WorkerCreateModel
    {
        public readonly Vector3 Position;
        public readonly Vector3 SpawnPoint;
        public readonly WorkerState State;
        public readonly Transform Parent;

        public WorkerCreateModel(
            WorkerState state,
            Vector3 spawnPoint,
            Vector3 position,
            Transform parent
        )
        {
            this.Position = position;
            this.SpawnPoint = spawnPoint;
            this.State = state;
            this.Parent = parent;
        }
    }
}