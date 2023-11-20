using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMain.Task
{
    public class BombEffectEvent : MonoBehaviour
    {
        private static readonly int Bomb1 = Animator.StringToHash("Bomb");
        [SerializeField] private Animator animator;
        private bool isBombEffectStop;

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