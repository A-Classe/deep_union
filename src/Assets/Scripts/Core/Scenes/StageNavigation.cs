using Core.User;

namespace Core.Scenes
{
    
    public enum StageNavigation
    {
        Tutorial,
        Stage1,
        Back
    }

    public static class StageNavEx
    {
        public static StageData.Stage ToStage(this StageNavigation nav)
        {
            return nav switch
            {
                StageNavigation.Stage1 => StageData.Stage.Stage1,
                StageNavigation.Tutorial => StageData.Stage.Tutorial,
                _ => StageData.Stage.Stage1
            };
        }
    }
}