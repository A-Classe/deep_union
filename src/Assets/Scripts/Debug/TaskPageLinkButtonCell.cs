using System;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace Debug
{
    public sealed class TaskPageLinkButtonCell : Cell<TaskPageLinkButtonCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private RectTransform _contents;
        [SerializeField] private CanvasGroup _contentsCanvasGroup;

        public CellIcon icon;
        public CellTexts cellTexts;
        public Image arrow;
        public Button button;

        protected override void SetModel(TaskPageLinkButtonCellModel model)
        {
            var pageType = model.PageType ?? typeof(DebugPage);

            _contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;

            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            cellTexts.Setup(model.CellTexts);

            // Arrow
            arrow.gameObject.SetActive(model.ShowArrow);

            // Button
            button.interactable = model.Interactable;
            button.colors = model.ColorBlock;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (model.Prefab == null)
                {
                    DebugSheet.Of(transform)
                        .PushPage(pageType, true, onLoad: x => model.InvokeOnLoad(x.pageId, x.page),
                            titleOverride: model.PageTitleOverride, pageId: model.PageId);
                }
                else
                {
                    DebugSheet.Of(transform)
                        .PushPage(pageType, model.Prefab, true,
                            onLoad: x => model.InvokeOnLoad(x.pageId, x.page),
                            titleOverride: model.PageTitleOverride, pageId: model.PageId);
                }
            });

            // Height
            var height = model.UseSubTextOrIcon ? 68 : 32; // Texts
            height += 26; // Padding
            height += 1; // Border
            _contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _layoutElement.preferredHeight = height; // Set the preferred height for the recycler view.
        }
    }

    public sealed class TaskPageLinkButtonCellModel : CellModel
    {
        public TaskPageLinkButtonCellModel(bool useSubTextOrIcon)
        {
            UseSubTextOrIcon = useSubTextOrIcon;
        }

        public string PageTitleOverride { get; set; }

        public CellIconModel Icon { get; } = new();

        public bool UseSubTextOrIcon { get; }

        public CellTextsModel CellTexts { get; } = new();

        public bool ShowArrow { get; set; }

        public bool Interactable { get; set; } = true;

        public Type PageType { get; set; }

        public DebugPageBase Prefab { get; set; }

        public string PageId { get; set; }

        public ColorBlock ColorBlock { get; set; }

        public event Action<(string pageId, DebugPageBase page)> OnLoad;

        internal void InvokeOnLoad(string pageId, DebugPageBase debugPage)
        {
            OnLoad?.Invoke((pageId, debugPage));
        }
    }
}