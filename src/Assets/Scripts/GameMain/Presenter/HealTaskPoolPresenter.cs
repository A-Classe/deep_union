﻿using Module.Assignment.System;
using Module.Task;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class HealTaskPoolPresenter : IInitializable
    {
        [Inject]
        public HealTaskPoolPresenter(HealTaskPool healTaskPool, ActiveAreaCollector areaCollector)
        {
            healTaskPool.OnHealTaskDrop += areaCollector.ActivateArea;
            healTaskPool.OnHealTaskCollected += areaCollector.DeactivateArea;
        }

        public void Initialize() { }
    }
}