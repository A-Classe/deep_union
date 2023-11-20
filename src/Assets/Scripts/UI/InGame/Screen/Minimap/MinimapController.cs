using System.Collections.Generic;
using UnityEngine;

namespace UI.InGame.Screen.Minimap
{
    [RequireComponent(typeof(Camera))]
    public class MinimapController: MonoBehaviour
    {
        private enum ControlState
        {
            MoveToMiniView,
            MoveToFocus,
            Idle
        }

        [SerializeField] private RectTransform rootUI;
        [SerializeField] private RectTransform mapRenderer;
        private ControlState moveState;
        private MinimapState state;

        private Dictionary<MinimapState, InStateProfile> profiles = new()
        {
            { MinimapState.Focus, new InStateProfile
            {
                RootUIPosition = new Vector3(0f, 0f, 0f),
                RootUIScale = new Vector3(6f, 6f, 6f),
                MapScale = new Vector3(1f, 1f, 1f),
            } }
        };


        private void Start()
        {
            SetProfile(MinimapState.MiniView, new InStateProfile
            {
                MapScale = mapRenderer.localScale,
                RootUIPosition = rootUI.position,
                RootUIScale = rootUI.localScale
            });
        }

        private void Update()
        {
            
        }

        internal void SetProfile(MinimapState key, InStateProfile profile)
        {
            profiles[key] = profile;
        }

        public void EnableFocusView()
        {
            state = MinimapState.Focus;
            UpdateInProfile();
        }

        public void DisableFocusView()
        {
            state = MinimapState.MiniView;
            UpdateInProfile();
        }

        internal MinimapState GetState() => state;

        private void UpdateInProfile()
        {
            rootUI.position = profiles[state].RootUIPosition;
            rootUI.localScale = profiles[state].RootUIScale;
            mapRenderer.localScale = profiles[state].MapScale;
        }
    }
}