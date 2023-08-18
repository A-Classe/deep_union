using System;
using UnityEngine;
using VContainer;
using Module.Worker;
using Module.Worker.Factory;
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

    public Module.Worker.Worker CreateWorker(WorkerCreateModel model)
    {
        if (prefab == null)
        {
            throw new NotImplementedException($"Prefab not found at path ({ArgCharacterPath})");
        }

        GameObject instance = Object.Instantiate(prefab, model.Position, Quaternion.identity, model.Parent);
        Module.Worker.Worker worker= instance.GetComponent<Module.Worker.Worker>();

        worker.OnSpawn(model.SpawnPoint);
        
        return worker;
    }
}