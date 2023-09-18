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

            float radiusA = math.max(ellipseA.Size.x, ellipseA.Size.y);
            float radiusB = math.max(ellipseB.Size.x, ellipseB.Size.y);

            return distance <= radiusA * radiusA + radiusB * radiusB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InEllipse(in float3 pos, in EllipseData ellipseData)
        {
            var dx = (ellipseData.Position.x - pos.x) / (ellipseData.Size.x * 0.5f);
            var dz = (ellipseData.Position.z - pos.z) / (ellipseData.Size.y * 0.5f);

            return (dx * dx + dz * dz) <= 1;
        }
    }
}