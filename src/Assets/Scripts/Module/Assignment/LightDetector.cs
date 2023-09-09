using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using Wanna.DebugEx;

namespace GameMain.System
{
    [BurstCompile]
    public class LightDetector
    {
        private readonly WorkerAgent workerAgent;
        private List<Transform> origins;

        public readonly List<AssignableArea> AssignableAreas;

        public event Action<Worker, AssignableArea> OnAssignableAreaDetected;

        [Inject]
        public LightDetector(TaskActivator taskActivator, WorkerController workerController, WorkerAgent workerAgent)
        {
            this.workerAgent = workerAgent;
            AssignableAreas = new List<AssignableArea>();
        }

        public void UpdateDetection()
        {
            var workerPositions = new NativeArray<float3>(workerAgent.ActiveWorkers.Length, Allocator.TempJob);
            var lightDataList = new NativeArray<LightData>(AssignableAreas.Count, Allocator.TempJob);
            var result = new NativeArray<int>(workerAgent.ActiveWorkers.Length, Allocator.TempJob);

            for (int i = 0; i < workerAgent.ActiveWorkers.Length; i++)
            {
                Worker worker = workerAgent.ActiveWorkers[i];
                workerPositions[i] = worker.transform.position;
            }

            for (int i = 0; i < AssignableAreas.Count; i++)
            {
                lightDataList[i] = AssignableAreas[i].LightData;
            }

            EllipseCollideJob job = new EllipseCollideJob()
            {
                WorkerPositions = workerPositions,
                LightDataList = lightDataList,
                Result = result
            };

            JobHandle handle = job.Schedule(workerAgent.ActiveWorkers.Length, 0);
            handle.Complete();

            for (int i = 0; i < result.Length; i++)
            {
                AssignableArea assignableArea = AssignableAreas[result[i]];
                Worker worker = workerAgent.ActiveWorkers[i];

                if (worker.AreaTarget == assignableArea.transform)
                    continue;

                OnAssignableAreaDetected?.Invoke(worker, assignableArea);
            }

            workerPositions.Dispose();
            lightDataList.Dispose();
            result.Dispose();
        }

        [BurstCompile]
        private struct EllipseCollideJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float3> WorkerPositions;
            [ReadOnly] public NativeArray<LightData> LightDataList;

            public NativeArray<int> Result;

            public void Execute(int index)
            {
                int result = -1;
                int collideCount = 0;
                var workerPos = WorkerPositions[index];

                for (var i = 0; i < LightDataList.Length; i++)
                {
                    var lightData = LightDataList[i];

                    if (lightData.Intensity <= 0f)
                        continue;

                    if (InEllipse(workerPos, lightData))
                    {
                        collideCount++;
                    }

                    if (InEllipse(workerPos, lightData) && CanTransfer(i, result))
                    {
                        result = i;
                    }
                }

                if (result == -1)
                {
                    float maxIntensity = LightDataList[0].Intensity;
                    result = 0;

                    for (int i = 1; i < LightDataList.Length; i++)
                    {
                        float intensity = LightDataList[i].Intensity;
                        if (intensity > maxIntensity)
                        {
                            result = i;
                            maxIntensity = intensity;
                        }
                    }
                }

                Result[index] = result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool InEllipse(in float3 pos, in LightData lightData)
            {
                var dx = (lightData.Position.x - pos.x) / (lightData.Size.x * 0.5f);
                var dz = (lightData.Position.z - pos.z) / (lightData.Size.y * 0.5f);

                return (dx * dx + dz * dz) <= 1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool CanTransfer(int index, int result)
            {
                if (result == -1)
                    return true;

                var lightData = LightDataList[index];
                var prevLightData = LightDataList[result];
                var isPrioritize = lightData.Intensity == prevLightData.Intensity && lightData.Priority > prevLightData.Priority;
                var isBrightest = lightData.Intensity > prevLightData.Intensity;

                return isPrioritize || isBrightest;
            }
        }
    }
}