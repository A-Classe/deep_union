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
                speed = param.PlayerSpeed
            };
        }
    }
}