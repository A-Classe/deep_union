using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace Module.Assignment.Utility
{
    [BurstCompile]
    public static class CollisionUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCollideCircle(in EllipseData ellipseA, in EllipseData ellipseB)
        {
            float distance = math.distancesq(ellipseA.Position, ellipseB.Position);

            float2 sizeA = ellipseA.Size * 0.5f;
            float2 sizeB = ellipseB.Size * 0.5f;

            float radiusA = math.max(sizeA.x, sizeA.y);
            float radiusB = math.max(sizeB.x, sizeB.y);

            return distance <= radiusA * radiusA + radiusB * radiusB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InEllipse(in float3 pos, in EllipseData ellipseData)
        {
            var dx = pos.x - ellipseData.Position.x;
            var dz = pos.z - ellipseData.Position.z;

            float rad = math.radians(ellipseData.Rotation);
            float sin = math.sin(rad);
            float cos = math.cos(rad);

            float sizeX = ellipseData.Size.x * 0.5f;
            float sizeY = ellipseData.Size.y * 0.5f;

            float x = dx * cos - dz * sin;
            float y = (sizeX / sizeY) * (-dx * sin - dz * cos);


            return (x * x + y * y) < sizeX * sizeX;
        }
    }
}