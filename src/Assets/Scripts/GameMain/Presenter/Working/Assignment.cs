using System;
using System.Collections.Generic;
using Module.Task;
using Module.Working;
using UnityEngine;

namespace GameMain.Presenter.Working
{
    public readonly struct Assignment : IEquatable<Assignment>
    {
        public readonly BaseTask Task;
        public readonly Transform Target;
        public readonly List<Worker> Workers;

        public Assignment(BaseTask task, Transform target)
        {
            Task = task;
            Target = target;
            Workers = new List<Worker>(64);
        }

        public bool Equals(Assignment other)
        {
            return Task == other.Task && Workers == other.Workers;
        }

        public override bool Equals(object obj)
        {
            return obj is Assignment other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Task, Workers);
        }
    }
}