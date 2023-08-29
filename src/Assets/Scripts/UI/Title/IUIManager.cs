using UnityEngine;

namespace UI.Title
{
    public interface IUIManager
    {
        void Initialized();

        void Select(Vector2 direction);

        void Clicked();

        void Finished();
    }
}