using System.GameProgress;
using Module.GameProgress;
using Module.Working.Controller;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class CompassPresenter : IInitializable
    {
        [Inject]
        public CompassPresenter(WorkerController workerController, GoalPoint goalPoint, Compass compass)
        {
            compass.SetOriginAndTarget(workerController.Target, goalPoint.transform);
        }

        public void Initialize() { }
    }
}