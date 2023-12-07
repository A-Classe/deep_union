using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Module.Extension.Task
{
    public class Enemy1TaskDirector : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int CanBomb = Animator.StringToHash("CanBomb");
        private static readonly int BodyColor = Shader.PropertyToID("_Color");

        [Header("ダメージとスケールの比例関数")]
        [SerializeField]
        private AnimationCurve damageScaleCurve;

        [Header("ダメージと点滅の比例関数")]
        [SerializeField]
        private AnimationCurve damageBlinkCurve;
        
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private Animator animator;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private BombEffectEvent bombEffectEvent;
        [SerializeField] private Transform scaleBody;
        [SerializeField] private Color blinkColor;
        [SerializeField] private float disableBodyDelay;
        
        private Tween blinkTween;
        private Material bodyMaterial;
        private Vector3 initialScale;

        private void Start()
        {
            bodyMaterial = bodyRenderer.material;
            decalProjector.enabled = false;
            initialScale = transform.localScale;
        }

        public void UpdateBlinkColor(float progress)
        {
            if (blinkTween == null)
                blinkTween = bodyMaterial.DOColor(blinkColor, BodyColor, 1f).SetLoops(-1, LoopType.Yoyo).Play();

            blinkTween.timeScale = damageBlinkCurve.Evaluate(progress);
        }
        
        public void UpdateScale(float progress)
        {
            scaleBody.localScale = initialScale * damageScaleCurve.Evaluate(progress);
        }

        public void EnableMovingState()
        {
            animator.SetBool(IsWalking, true);
            decalProjector.enabled = true;
        }

        public async UniTask AnimateExplode()
        {
            //爆破モーション開始
            animator.SetBool(IsWalking, false);
            animator.SetTrigger(CanBomb);
            
            await UniTask.Delay(TimeSpan.FromSeconds(disableBodyDelay));

            animator.gameObject.SetActive(false);
        }

        public async UniTask EffectExplode()
        {
            await bombEffectEvent.Bomb();
        }
    }
}