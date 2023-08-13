using UnityEngine;

namespace Worker.Factory
{
    public readonly struct WorkerCreateModel
    {
        public readonly Vector3 Position;
        public readonly WorkerState State;
        public readonly Transform Parent;

        public WorkerCreateModel(
            WorkerState state,
            Vector3 position,
            Transform parent
        )
        {
            this.Position = position;
            this.State = state;
            this.Parent = parent;
        }
    }
}