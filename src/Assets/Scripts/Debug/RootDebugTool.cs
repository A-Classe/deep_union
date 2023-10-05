using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Extensions.Graphy;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Debug
{
    public class RootDebugTool : IStartable
    {
        private readonly DebugSheet debugSheet;
        private readonly DebugToolPage debugToolPage;

        [Inject]
        public RootDebugTool(DebugSheet debugSheet)
        {
            this.debugSheet = debugSheet;
            debugToolPage = debugSheet.GetOrCreateInitialPage<DebugToolPage>();
        }

        public void Start()
        {
            debugToolPage.AddPageLinkButton<GraphyDebugPage>("Graphy",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.FPS),
                onLoad: x => x.page.Setup(GraphyManager.Instance), priority: 100);
            debugSheet.GetComponent<Canvas>().enabled = true;

            debugToolPage.Reload();
        }
    }
}