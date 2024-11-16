using System.GameProgress;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class GoalTaskCompilationPresenter : IInitializable,ITickable
    {
        private readonly StageProgressObserver progressObserver;

        [Inject]
        public GoalTaskCompilationPresenter(GoalTaskCompilationObserver compilationObserver,StageProgressObserver progressObserver)
        {
            this.progressObserver = progressObserver;
            compilationObserver.OnCompleted += progressObserver.ForceComplete;
        }
        
        public void Initialize() { }
        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                progressObserver.ForceComplete();
            }
        }
    }
}