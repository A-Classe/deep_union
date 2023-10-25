using Module.Task;
using UI.InGame;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace GameMain.Presenter.Resource
{
    /// <summary>
    /// タスクとリソース管理クラスのプレゼンタークラス
    /// </summary>
    public class ResourcePresenter : IInitializable
    {
        private readonly ResourceContainer resourceContainer;
        private readonly InGameUIManager uiManager;
        private readonly CollectableTask[] collectableTasks;

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
                    DebugEx.Log($"ResourceCount: {resourceContainer.ResourceCount}");
                };
            }
            
            uint initialResourceCount = resourceContainer.MaxResourceCount > 0 ? (uint)resourceContainer.MaxResourceCount : 0u;
            uiManager.SetResourceCount(0u, initialResourceCount);
            resourceContainer.OnResourceChanged += (_, current) =>
            {
                uiManager.SetResourceCount((uint)current);
            };
        }
    }
}