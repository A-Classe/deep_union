using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private float dequeueInterval;

    private Queue<Vector3> queue;
    private Vector3[] positions;

    private Camera cachedCamera;
    private CancellationToken destroyCanceller;

    const int capacity = 100;

    [SerializeField] private LineRenderer line;

    void Awake()
    {
        positions = new Vector3[capacity];
        queue = new Queue<Vector3>();
        cachedCamera = Camera.main;
        destroyCanceller = this.GetCancellationTokenOnDestroy();

        DelayKill().Forget();
    }

    public void AddPosition(Vector2 pos)
    {
        var targetPos = cachedCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10));
        queue.Enqueue(targetPos);
        if (queue.Count > capacity)
            queue.Dequeue();

        if (queue.Count != 0)
            Draw();
    }

    private async UniTaskVoid DelayKill()
    {
        while (!destroyCanceller.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(dequeueInterval), cancellationToken: destroyCanceller);

            if (queue.Count > 0)
            {
                queue.Dequeue();
                Draw();
            }
        }
    }

    void Draw()
    {
        queue.CopyTo(positions, 0);
        line.positionCount = queue.Count;
        line.SetPositions(positions);
    }
}