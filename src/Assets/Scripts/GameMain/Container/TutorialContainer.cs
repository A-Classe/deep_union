using System.Collections;
using System.Collections.Generic;
using GameMain.Tutorial;
using Module.GameManagement;
using UnityEngine;
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