using Cysharp.Threading.Tasks;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Extension.Task
{
    public class BombEffectEvent : MonoBehaviour
    {
        private static readonly int Bomb1 = Animator.StringToHash("Bomb1");
        [SerializeField] private Animator animator;
        private bool isBombEffectStop;

        public async UniTask Bomb()
        {
            animator.gameObject.SetActive(true);
            animator.Play(Bomb1);

            await UniTask.WaitUntil(() => isBombEffectStop);
        }

        public void OnBombEffectStopEvent()
        {
            isBombEffectStop = true;
        }
    }
}