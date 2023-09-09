using System.GameProgress;
using Module.Assignment;
using Module.Task;
using Module.Working.Controller;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Debug
{
    public sealed class SceneDebugTool : IStartable
    {
        private readonly ResourceContainer resourceContainer;
        private readonly LeaderAssignEvent leaderAssignEvent;
        private readonly StageProgressObserver stageProgressObserver;

        [Inject]
        public SceneDebugTool(ResourceContainer resourceContainer, LeaderAssignEvent leaderAssignEvent, StageProgressObserver stageProgressObserver)
        {
            this.resourceContainer = resourceContainer;
            this.leaderAssignEvent = leaderAssignEvent;
            this.stageProgressObserver = stageProgressObserver;
        }

        public void Start()
        {
            DebugToolPage initialPage = DebugSheet.Instance.GetOrCreateInitialPage<DebugToolPage>();

            initialPage.SetUp(stageProgressObserver);

            initialPage.AddPageLinkButton<TaskDebugPage>("Task",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Model));
            initialPage.AddPageLinkButton<PlayerDebugPage>("Player",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.CharacterViewer), onLoad: data =>
                {
                    data.page.SetUp(resourceContainer, leaderAssignEvent);
                });

            initialPage.Reload();
        }
    }
}