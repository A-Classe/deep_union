using System;
using UnityEngine;
using VContainer;
using Worker;
using Worker.Factory;
using Object = UnityEngine.Object;

public class WorkerFactory
{
    private static readonly string ArgCharacterPath = "Worker";
    private readonly GameObject prefab;

    [Inject]
    public WorkerFactory()
    {
        prefab = Resources.Load<GameObject>(ArgCharacterPath);
    }

    public TaskWorker CreateWorker(WorkerCreateModel model)
    {
        if (prefab == null)
        {
            throw new NotImplementedException($"Prefab not found at path ({ArgCharacterPath})");
        }

        GameObject instance = Object.Instantiate(prefab, model.Position, Quaternion.identity, model.Parent);
        return instance.GetComponent<TaskWorker>();
    }
}