using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Module.Task
{
    [BurstCompile]
    [Serializable]
    public readonly struct LightData
    {
        public readonly float3 Position;
        public readonly float2 Size;
        public readonly float Intensity;
        public readonly short Priority;

        public LightData(float3 position, float2 size, float intensity, short priority)
        {
            Position = position;
            Size = size;
            Intensity = intensity;
            Priority = priority;
        }
    }
}