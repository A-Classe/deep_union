using UnityEngine;

namespace Core.Model.Player
{
    public struct PlayerInitModel
    {
        // ReSharper disable once UnassignedField.Global
        public Vector3? startPosition;

        public float? accel;
        public float? maxSpeed;
    }

    public struct PlayerStatusModel
    {
        public short? hp;

        public short? maxHp;
    }
}