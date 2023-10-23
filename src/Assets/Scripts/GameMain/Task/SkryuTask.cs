using System.Collections;
using System.Collections.Generic;
using Module.Task;
using UnityEngine;

namespace GameMain.Task
{
    public class SkryuTask : BaseTask
    {
        [SerializeField] private float skryuSpeed;
        [SerializeField] private GameObject beforeObject;
        [SerializeField] private GameObject afterObject;
        [SerializeField] private Animator featherAnimator;

        static readonly int Rotating = Animator.StringToHash("IsRotating");

        protected override void OnComplete()
        {
            beforeObject.SetActive(false);
            afterObject.SetActive(true);

            featherAnimator.SetBool(Rotating, true);
            featherAnimator.speed = skryuSpeed;
        }
    }
}