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
            var distance = math.distancesq(ellipseA.Position, ellipseB.Position);

            var sizeA = ellipseA.Size * 0.5f;
            var sizeB = ellipseB.Size * 0.5f;

            var radiusA = math.max(sizeA.x, sizeA.y);
            var radiusB = math.max(sizeB.x, sizeB.y);

            return distance <= radiusA * radiusA + radiusB * radiusB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InEllipse(in float3 pos, in EllipseData ellipseData)
        {
            var dx = pos.x - ellipseData.Position.x;
            var dz = pos.z - ellipseData.Position.z;

            var rad = math.radians(ellipseData.Rotation);
            var sin = math.sin(rad);
            var cos = math.cos(rad);

            var sizeX = ellipseData.Size.x * 0.5f;
            var sizeY = ellipseData.Size.y * 0.5f;

            var x = dx * cos - dz * sin;
            var y = sizeX / sizeY * (-dx * sin - dz * cos);


            return x * x + y * y < sizeX * sizeX;
        }
    }
}