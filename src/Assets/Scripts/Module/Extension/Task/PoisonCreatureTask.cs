using System;
using Cysharp.Threading.Tasks;
using Module.Task;
using UnityEngine;
using VContainer;

namespace Module.Extension.Task
{
    public class PoisonCreatureTask : BaseTask
    {
        private static readonly int IsDead = Animator.StringToHash("Dead");
        [SerializeField] private Animator animator;
        [SerializeField] private float DeadAnimationTime;

        public override void Initialize(IObjectResolver container)
        {
            animator.SetBool(IsDead, false);
        }

        protected override async void OnComplete()
        {
            animator.SetBool(IsDead, true);
            await UniTask.Delay(TimeSpan.FromSeconds(DeadAnimationTime));
            Disable();
        }
    }
}