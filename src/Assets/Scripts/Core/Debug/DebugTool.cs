#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Extensions.Graphy;
using UnityEngine;

namespace Core.Debug
{
    public sealed class DebugTool : MonoBehaviour
    {
        private static DebugTool instance;

        public GraphyManager graphyManager;

        private DefaultDebugPageBase initialPage;
        private int linkButtonId = -1;

        public static DebugTool Instance
        {
            get
            {
                if (instance == null)
                    throw new InvalidOperationException("The singleton instance of the DebugTools does not exits.");
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            Destroy(gameObject);
        }

        private void Start()
        {
            initialPage = DebugSheet.Instance.GetOrCreateInitialPage<DebugToolPage>();
            initialPage.AddPageLinkButton<GraphyDebugPage>("Graphy",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.FPS),
                onLoad: x => x.page.Setup(GraphyManager.Instance));
            initialPage.Reload();
        }

        private void OnDestroy() { }
    }
}
#endif