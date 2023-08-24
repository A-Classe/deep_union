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
            Position = position;
            SpawnPoint = parent.position;
            State = state;
            Parent = parent;
        }
    }
}