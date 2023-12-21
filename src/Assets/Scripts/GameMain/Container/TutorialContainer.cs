using GameMain.Tutorial;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class TutorialContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TutorialSequencer>();
        }
    }
}