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
            DebugEx.Log("Explode Start");

            await UniTask.WaitUntil(() => isBombEffectStop);

            DebugEx.Log("Explode ENd");
        }

        public void OnBombEffectStopEvent()
        {
            isBombEffectStop = true;
        }
    }
}