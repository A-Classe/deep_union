using GameMain.Presenter;
using UnityEngine;

namespace Core.Model.Player
{
    public struct PlayerInitModel
    {
        // ReSharper disable once UnassignedField.Global
        public Vector3? startPosition;

        public float? speed;
    }

    public struct PlayerStatusModel
    {
        public short? hp;

        public short? maxHp;
        
    }
}