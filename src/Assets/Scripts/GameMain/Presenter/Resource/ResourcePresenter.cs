using Module.Extension.Task;
using Module.Task;
using Module.UI.InGame;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter.Resource
{
    /// <summary>
    ///     タスクとリソース管理クラスのプレゼンタークラス
    /// </summary>
    public class ResourcePresenter : IInitializable
    {
        private readonly CollectableTask[] collectableTasks;
        private readonly ResourceContainer resourceContainer;
        private readonly InGameUIManager uiManager;

        [Inject]
        public ResourcePresenter(
            ResourceContainer resourceContainer,
            InGameUIManager uiManager
        )
        {
            this.resourceContainer = resourceContainer;
            this.uiManager = uiManager;
            collectableTasks = TaskUtil.FindSceneTasks<CollectableTask>();
        }

        public void Initialize()
        {
            foreach (var collectableTask in collectableTasks)
            {
                collectableTask.OnCollected += count =>
                {
                    resourceContainer.Add(count);
                };
            }

            var initialResourceCount =
                resourceContainer.MaxResourceCount > 0 ? (uint)resourceContainer.MaxResourceCount : 0u;
            uiManager.SetResourceCount(0u, initialResourceCount);
            resourceContainer.OnResourceChanged += (_, current) =>
            {
                uiManager.SetResourceCount((uint)current);
            };
        }
    }
}