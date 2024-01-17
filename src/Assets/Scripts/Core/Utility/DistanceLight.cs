using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Utility
{
    /// <summary>
    /// カメラからの距離に応じてカメラの有効化を制御するクラス
    /// </summary>
    public class DistanceLight : MonoBehaviour
    {
        [SerializeField] private Light myLight;
        [SerializeField] [Range(0, 1)] private float updateInterval = 0.1f;
        [SerializeField] private float allowDistance = 5f;

        private Transform camTransform;
        private bool isDisappeared;

        private void Awake()
        {
            camTransform = Camera.main.transform;
            DoLightControl().Forget();
        }

        private async UniTaskVoid DoLightControl()
        {
            CancellationToken destroyCanceller = this.GetCancellationTokenOnDestroy();

            while (!destroyCanceller.IsCancellationRequested)
            {
                float distanceFromCamera = (camTransform.position - transform.position).sqrMagnitude;

                //指定距離内に入っていたら有効化
                if (distanceFromCamera <= allowDistance)
                {
                    myLight.enabled = true;
                    isDisappeared = false;
                }
                else if (!isDisappeared)
                {
                    myLight.enabled = false;
                    isDisappeared = true;
                }

                //更新間隔を待機
                await UniTask.Delay(TimeSpan.FromSeconds(updateInterval), cancellationToken: destroyCanceller);
            }
        }
    }
}