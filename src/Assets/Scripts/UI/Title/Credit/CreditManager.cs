using UnityEngine;

namespace UI.Title.Credit
{
    internal class CreditManager : MonoBehaviour, IUIManager
    {
        
        public void Initialized()
        {     
            gameObject.SetActive(true);
        }

        public void Clicked()
        { }

        public void Select(Vector2 direction)
        { }

        public void Finished()
        {
            gameObject.SetActive(false);
        }
    }
}