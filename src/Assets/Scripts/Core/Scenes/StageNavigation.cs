using Core.User;

namespace Core.Scenes
{
    public enum StageNavigation
    {
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Back
    }

    public static class StageNavEx
    {
        public static StageData.Stage ToStage(this StageNavigation nav)
        {
            return nav switch
            {
                StageNavigation.Stage1 => StageData.Stage.Stage1,
                StageNavigation.Stage2 => StageData.Stage.Stage2,
                StageNavigation.Stage3 => StageData.Stage.Stage3,
                StageNavigation.Stage4 => StageData.Stage.Stage4,
                StageNavigation.Stage5 => StageData.Stage.Stage5,
                _ => StageData.Stage.Stage1
            };
        }
    }
}