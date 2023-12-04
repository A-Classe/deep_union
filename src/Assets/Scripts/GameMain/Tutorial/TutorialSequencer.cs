using Module.GameManagement;
using VContainer.Unity;

namespace GameMain.Tutorial
{
    public class TutorialSequencer : IStartable
    {
        private readonly TimeManager timeManager;

        public TutorialSequencer(TimeManager timeManager)
        {
            this.timeManager = timeManager;
        }

        public void Start()
        {
            //timeManager.Pause();
        }
    }
}