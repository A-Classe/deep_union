using Core.Model.Player;
using GameMain.Presenter;

namespace Core.Utility.Player
{
    public static class PlayerUtility
    {
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