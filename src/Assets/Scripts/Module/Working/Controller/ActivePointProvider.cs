using System.Collections.Generic;
using UnityEngine;

namespace Module.Working.Controller
{
    public class ActivePointProvider
    {
        private readonly Transform target;
        private readonly IReadOnlyList<Vector3> offsets;

        public ActivePointProvider(Transform target, IReadOnlyList<Vector3> offsets)
        {
            this.target = target;
            this.offsets = offsets;
        }

        struct Point
        {
            private Vector3 point;
            private Worker worker;
        }
    }
}