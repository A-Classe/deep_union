using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.Navigation
{
    public interface IUIManager
    {
        void Initialized(ContentTransform content);

        void Select(Vector2 direction);

        void Clicked();

        void Finished(ContentTransform content, Action onFinished);

        AnimationBehaviour GetContext();
    }
}