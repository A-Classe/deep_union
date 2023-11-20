using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Utility
{
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
            AdjustLODQuality().Forget();
        }

        private async UniTaskVoid AdjustLODQuality()
        {
            while (!this.GetCancellationTokenOnDestroy().IsCancellationRequested)
            {
                var squareDistanceFromCamera = (camTransform.position - transform.position).sqrMagnitude;

                if (squareDistanceFromCamera <= allowDistance)
                {
                    myLight.enabled = true;
                    isDisappeared = false;
                }
                else if (!isDisappeared)
                {
                    myLight.enabled = false;
                    isDisappeared = true;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(updateInterval));
            }
        }
    }
}