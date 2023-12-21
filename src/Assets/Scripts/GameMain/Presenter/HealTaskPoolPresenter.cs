using Module.Task;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class HealTaskPoolPresenter : IInitializable
    {
        [Inject]
        public HealTaskPoolPresenter(HealTaskPool healTaskPool, TaskActivator taskActivator)
        {
            healTaskPool.OnHealTaskDrop += taskActivator.ForceActivate;
        }

        public void Initialize() { }
    }
}