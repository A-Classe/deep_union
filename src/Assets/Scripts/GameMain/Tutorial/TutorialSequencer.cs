using Module.GameManagement;
using UnityEngine;
using VContainer.Unity;

namespace GameMain.Tutorial
{
    public class TutorialSequencer : IInitializable
    {
        private readonly TimeManager timeManager;
        private readonly ITutorialTrigger[] tutorialTriggers;

        public TutorialSequencer(TimeManager timeManager)
        {
            this.timeManager = timeManager;
            tutorialTriggers = GameObject.Find("TutorialEvent").GetComponentsInChildren<ITutorialTrigger>();
        }

        public void Initialize()
        {
            foreach (ITutorialTrigger tutorialTrigger in tutorialTriggers)
            {
                tutorialTrigger.OnShowText += Pause;
                tutorialTrigger.OnHideText += Resume;
            }
        }

        private void Pause()
        {
            timeManager.Pause();
        }

        private void Resume()
        {
            timeManager.Resume();
        }
    }
}