using System;

namespace GameMain.Tutorial
{
    public interface ITutorialTrigger
    {
        public event Action OnShowText;
        public event Action OnHideText;
    }
}