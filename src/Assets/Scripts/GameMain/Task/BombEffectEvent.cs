using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMain.Task
{
    public class BombEffectEvent : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private bool isBombEffectStop;
        private static readonly int Bomb1 = Animator.StringToHash("Bomb");

        public async UniTask Bomb()
        {
            animator.gameObject.SetActive(true);
            animator.SetTrigger(Bomb1);

            await UniTask.WaitUntil(() => isBombEffectStop);
        }

        public void OnBombEffectStopEvent()
        {
            isBombEffectStop = true;
        }
    }
}