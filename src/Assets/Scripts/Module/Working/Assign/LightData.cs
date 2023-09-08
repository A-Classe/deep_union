using Unity.Burst;
using Unity.Mathematics;

namespace Module.Task
{
    [BurstCompile]
    public readonly struct LightData
    {
        public readonly float Width;
        public readonly float Height;
        public readonly float Intensity;

        public LightData(float width, float height, float intensity)
        {
            Width = width;
            Height = height;
            Intensity = intensity;
        }
    }
}