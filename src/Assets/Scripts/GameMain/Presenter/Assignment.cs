using System;
using System.Collections.Generic;
using Module.Task;
using Module.Working;
using UnityEngine;

namespace GameMain.Presenter
{
    public readonly struct Assignment : IEquatable<Assignment>
    {
        public readonly IJobHandle JobHandle;
        public readonly Transform Target;
        public readonly List<Worker> Workers;

        public Assignment(IJobHandle jobHandle, Transform target)
        {
            JobHandle = jobHandle;
            Target = target;
            Workers = new List<Worker>(64);
        }

        public bool Equals(Assignment other)
        {
            return JobHandle == other.JobHandle && Workers == other.Workers;
        }

        public override bool Equals(object obj)
        {
            return obj is Assignment other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(JobHandle, Workers);
        }
    }
}