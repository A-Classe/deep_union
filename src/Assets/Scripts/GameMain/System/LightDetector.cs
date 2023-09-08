using System;
using System.Collections.Generic;
using GameMain.Presenter;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using NotImplementedException = System.NotImplementedException;

namespace GameMain.System
{
    public class LightDetector
    {
        private readonly TaskActivator taskActivator;
        private readonly WorkerAgent workerAgent;
        private readonly GameParam gameParam;
        private readonly int layerMask;
        private List<Transform> origins;

        private readonly List<AssignableArea> assignableAreas;

        [Inject]
        public LightDetector(TaskActivator taskActivator, WorkerAgent workerAgent, GameParam gameParam)
        {
            this.taskActivator = taskActivator;
            this.workerAgent = workerAgent;
            assignableAreas = new List<AssignableArea>();
        }

        private void InitializeTasks() { }

        public void UpdateDetection()
        {
            NativeArray<float3> workerPositions = new NativeArray<float3>(workerAgent.ActiveWorkers.Length, Allocator.TempJob);
            NativeArray<float3> lightPositions = new NativeArray<float3>(assignableAreas.Count, Allocator.TempJob);

            foreach (Worker worker in workerAgent.ActiveWorkers) { }
        }

        [BurstCompile]
        private struct EllipseCollideJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float3> WorkerPositions;
            [ReadOnly] public NativeArray<float3> LightPositions;
            [ReadOnly] public NativeArray<LightData> LightDataList;

            public NativeArray<int> Result;

            public void Execute(int index)
            {
                int result = -1;
                var workerPos = WorkerPositions[index];

                for (var i = 0; i < LightPositions.Length; i++)
                {
                    var lightPos = LightPositions[i];
                    var lightData = LightDataList[i];

                    if (InEllipse(workerPos, lightPos, lightData) && lightData.Intensity > LightDataList[result].Intensity)
                    {
                        result = i;
                    }
                }

                Result[index] = result;
            }

            private bool InEllipse(in float3 a, in float3 b, in LightData lightData)
            {
                var dx = (b.x - a.x) / (lightData.Size.x * 0.5f);
                var dz = (b.z - a.z) / (lightData.Size.y * 0.5f);

                return (dx * dx + dz * dz) <= 1;
            }
        }
    }
}