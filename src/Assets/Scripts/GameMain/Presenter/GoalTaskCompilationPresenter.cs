using System.GameProgress;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class GoalTaskCompilationPresenter : IInitializable
    {
        [Inject]
        public GoalTaskCompilationPresenter(GoalTaskCompilationObserver compilationObserver,StageProgressObserver progressObserver)
        {
            compilationObserver.OnCompleted += progressObserver.ForceComplete;
        }
        
        public void Initialize() { }
    }
}