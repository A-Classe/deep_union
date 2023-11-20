using Module.Task;
using UnityEngine;

namespace GameMain.Task
{
    public class SkryuTask : BaseTask
    {
        private static readonly int Rotating = Animator.StringToHash("IsRotating");
        [SerializeField] private float skryuSpeed;
        [SerializeField] private GameObject beforeObject;
        [SerializeField] private GameObject afterObject;
        [SerializeField] private Animator featherAnimator;

        protected override void OnComplete()
        {
            beforeObject.SetActive(false);
            afterObject.SetActive(true);

            featherAnimator.SetBool(Rotating, true);
            featherAnimator.speed = skryuSpeed;
        }
    }
}