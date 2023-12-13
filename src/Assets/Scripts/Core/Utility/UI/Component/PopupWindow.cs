using AnimationPro.RunTime;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Utility.UI.Component
{
    public class PopupWindow: MonoBehaviour
    {
        [SerializeField] private AnimateObject window;
        private ContentTransform EnterTransform { get; set; }
        private ContentTransform ExitTransform { get; set; }
        
        public bool IsVisible => window.gameObject.activeSelf;

        private RateSpec rateSpec = Easings.Default(0.3f);

        private void Awake()
        {
            EnterTransform = window.FadeIn(rateSpec);
            ExitTransform = window.FadeOut(rateSpec);
            window.gameObject.SetActive(false);
        }
        
        public void SetVisible(bool isVisible)
        {
            window.OnCancel();
            window.gameObject.SetActive(true);
            window.Animation(
                isVisible ? EnterTransform : ExitTransform, 
                new AnimationListener 
                {
                    OnStart = () =>
                    {
                        window.gameObject.SetActive(true);
                    },
                    OnFinished = () =>
                    {
                        window.gameObject.SetActive(isVisible);
                    }
                }
            );
        }
        
        
    }
}