using System.GameProgress;
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
        private readonly LeadPointConnector leadPointConnector;
        private readonly StageProgressObserver stageProgressObserver;

        [Inject]
        public SceneDebugTool(ResourceContainer resourceContainer, LeadPointConnector leadPointConnector, StageProgressObserver stageProgressObserver)
        {
            this.resourceContainer = resourceContainer;
            this.leadPointConnector = leadPointConnector;
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
                    data.page.SetUp(resourceContainer, leadPointConnector);
                });

            initialPage.Reload();
        }
    }
}