using Module.Working.State;
using UnityEngine;

namespace Module.Working.Factory
{
    public readonly struct WorkerCreateModel
    {
        public readonly Vector3 Position;
        public readonly Vector3 SpawnPoint;
        public readonly WorkerState State;
        public readonly Transform Parent;

        public WorkerCreateModel(
            WorkerState state,
            Vector3 position,
            Transform parent
        )
        {
            this.Position = position;
            this.SpawnPoint = parent.position;
            this.State = state;
            this.Parent = parent;
        }
    }
}