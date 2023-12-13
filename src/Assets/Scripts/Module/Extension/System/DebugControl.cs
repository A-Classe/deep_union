using Core.Input;
using Core.User;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Module.Extension.System
{
    public class DebugControl: IStartable
    {

        private float lastUpdateTime = 0f;
        private int inputCount = 0;

        private readonly UserPreference preference;
        [Inject]
        public DebugControl(
            UserPreference preference
        )
        {
            this.preference = preference;
        }

        public void Start()
        {
            var deleteKeyEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.UI.DebugQ);
            
            deleteKeyEvent.Started += _ =>
            {
                float current = Time.time;
                if (lastUpdateTime + 1f < current)
                {
                    inputCount = 0;   
                }
                inputCount++;
                lastUpdateTime = Time.time;
                if (inputCount > 5)
                {
                    inputCount = 0;
                    lastUpdateTime = 0f;
                    preference.ResetIsFirst();
                    preference.Load();
                    Debug.Log("userData delete & created");
                    
                }
            };
        }
    }
}