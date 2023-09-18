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
        [Inject]
        public RootDebugTool() { }

        public void Start()
        {
            DefaultDebugPageBase initialPage = DebugSheet.Instance.GetOrCreateInitialPage<DebugToolPage>();

            initialPage.AddPageLinkButton<GraphyDebugPage>("Graphy",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.FPS),
                onLoad: x => x.page.Setup(GraphyManager.Instance), priority: 100);
            DebugSheet.Instance.GetComponent<Canvas>().enabled = true;

            initialPage.Reload();
        }
    }
}