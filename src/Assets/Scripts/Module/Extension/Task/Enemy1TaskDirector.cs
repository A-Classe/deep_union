using System;
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
        [SerializeField] private Color blinkColor;
        
        private Tween blinkTween;
        private Material bodyMaterial;

        private void Start()
        {
            
        }

        private void UpdateBlinkColor(float progress)
        {
            if (blinkTween == null)
                blinkTween = bodyMaterial.DOColor(blinkColor, BodyColor, 1f).SetLoops(-1, LoopType.Yoyo).Play();

            blinkTween.timeScale = damageBlinkCurve.Evaluate(progress);
        }
        
        
        private void UpdateScale(float progress)
        {
            scaleBody.localScale = initialScale * damageScaleCurve.Evaluate(progress);
        }
    }
}