using System;
using System.Collections.Generic;
using Module.Extension.Task;
using UnityEngine;

public class HealTaskPool : MonoBehaviour
{
    private Queue<HealTask> taskPool;
    private HealTask[] returnCache;

    public event Action<HealTask> OnHealTaskDrop;

    private void Start()
    {
        taskPool = new Queue<HealTask>(GetComponentsInChildren<HealTask>(true));
        returnCache = new HealTask[16];
    }

    public Span<HealTask> Get(int count)
    {
        if (taskPool.Count - count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        for (int i = 0; i < count; i++)
        {
            HealTask healTask = taskPool.Dequeue();
            returnCache[i] = healTask;
            healTask.OnCollected += Return;
            OnHealTaskDrop?.Invoke(healTask);
        }

        return returnCache.AsSpan(0, count);
    }

    public void Return(HealTask healTask)
    {
        healTask.OnCollected -= Return;
        taskPool.Enqueue(healTask);
    }
}