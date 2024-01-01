using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.Extension.Task
{
    public class ResourceItem : MonoBehaviour
    {
        public async UniTaskVoid Throw(Transform target, Vector3 offset, Vector3 initialForce, Action onCollide)
        {
            float duration = (target.position - transform.position).magnitude / initialForce.magnitude;
            await ParaboloidLoop(target, transform.position, offset, initialForce, duration);

            onCollide?.Invoke();
            Destroy(gameObject);
        }

        private async UniTask ParaboloidLoop(Transform target, Vector3 position, Vector3 offset, Vector3 velocity, float duration)
        {
            CancellationToken cToken = this.GetCancellationTokenOnDestroy();
            float period = duration;

            while (!cToken.IsCancellationRequested)
            {
                //FixedUpdateと同期
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cToken);

                Vector3 acceleration = Vector3.zero;
                Vector3 diff = target.position + offset - position;
                acceleration += (diff - velocity * period) * 2f / (period * period);

                period -= Time.fixedDeltaTime;
                if (period < 0f)
                {
                    return;
                }

                //運動方程式に従って移動
                velocity += acceleration * Time.fixedDeltaTime;
                position += velocity * Time.fixedDeltaTime;
                transform.position = position;
            }
        }
    }
}