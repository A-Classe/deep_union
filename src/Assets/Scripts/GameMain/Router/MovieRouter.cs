using Core.Scenes;
using Module.Extension.UI;
using VContainer;
using VContainer.Unity;

namespace GameMain.Router
{
    public class MovieRouter: IStartable
    {
        private readonly VideoPlayerController videoController;

        private readonly SceneChanger sceneChanger;
        [Inject]
        public MovieRouter(
            SceneChanger sceneChanger,
            VideoPlayerController videoController
        )
        {
            this.sceneChanger = sceneChanger;
            this.videoController = videoController;
        }
        public void Start()
        {
            videoController.Play(() =>
            {
                sceneChanger.Next();
            });
            // InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release) 
        }
    }
}