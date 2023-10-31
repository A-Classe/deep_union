using Core.Model.Player;
using GameMain.Presenter;

namespace Core.Utility.Player
{
    public static class PlayerUtility
    {
        public static PlayerInitModel ConvertToPlayerModel(this GameParam　param)
        {
            return new PlayerInitModel
            {
                accel = param.MoveAccelaration,
                maxSpeed = param.MaxSpeed
            };
        }

        public static PlayerStatusModel ConvertToStatus(this GameParam param)
        {
            return new PlayerStatusModel
            {
                hp = (short)param.HitPoint,
                maxHp = (short)param.HitPoint
            };
        }
    }
}