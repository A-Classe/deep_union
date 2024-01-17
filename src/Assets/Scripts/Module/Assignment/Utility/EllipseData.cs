using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Module.Assignment.Utility
{
    [Serializable]
    public readonly struct EllipseData
    {
        public readonly float3 Position;
        public readonly float Rotation;
        public readonly float2 Size;

        public EllipseData(float3 position, float2 size, float rotation)
        {
            Position = position;
            Size = size;
            Rotation = rotation;
        }
    }
}