using UnityEngine;

namespace UI.Title.Option4
{
    public class Option4Manager: MonoBehaviour, IUIManager
    {
        private void Awake()
        {
        }


        public void Initialized()
        {     
            gameObject.SetActive(true);
        }
        public void Clicked()
        { }

        public void Select(Vector2 direction)
        {
        }

        public void Finished()
        {
            gameObject.SetActive(false);
        }
    }
}