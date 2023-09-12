using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Module.Task
{
    [BurstCompile]
    [Serializable]
    public readonly struct LightData
    {
        public readonly float2 Size;
        public readonly float Intensity;

        public LightData(float2 size, float intensity)
        {
            Size = size;
            Intensity = intensity;
        }
    }
}