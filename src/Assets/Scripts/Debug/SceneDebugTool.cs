using System.GameProgress;
using Module.Assignment.Component;
using Module.Task;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Debug
{
    public sealed class SceneDebugTool : IStartable
    {
        private readonly DebugToolPage debugToolPage;
        private readonly LeaderAssignableArea leaderAssignableArea;
        private readonly ResourceContainer resourceContainer;
        private readonly StageProgressObserver stageProgressObserver;

        [Inject]
        public SceneDebugTool(
            ResourceContainer resourceContainer,
            LeaderAssignableArea leaderAssignableArea,
            StageProgressObserver stageProgressObserver,
            DebugSheet debugSheet
        )
        {
            this.resourceContainer = resourceContainer;
            this.leaderAssignableArea = leaderAssignableArea;
            this.stageProgressObserver = stageProgressObserver;
            debugToolPage = debugSheet.GetOrCreateInitialPage<DebugToolPage>();
        }

        public void Start()
        {
            debugToolPage.SetUp(stageProgressObserver);

            debugToolPage.AddPageLinkButton<TaskDebugPage>("Task",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Model));
            debugToolPage.AddPageLinkButton<PlayerDebugPage>("Player",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.CharacterViewer),
                onLoad: data => { data.page.SetUp(resourceContainer, leaderAssignableArea); });

            debugToolPage.Reload();
        }
    }
}