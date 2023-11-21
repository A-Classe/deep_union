using System.Collections.Generic;
using UnityEngine;

namespace Module.UI.InGame.Minimap
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
        [SerializeField] private Transform player;
        [SerializeField] private RectTransform center;
        private ControlState moveState;
        private MinimapState state;
        
        private Camera renderCamera;

        private Dictionary<MinimapState, InStateProfile> profiles = new()
        {
            { MinimapState.Focus, new InStateProfile
            {
                RootPositionAnchor = new Vector3(0f, 0f, 0f),
                RootUIScale = new Vector3(6f, 6f, 6f),
                MapScale = new Vector3(1f, 1f, 1f),
            } }
        };


        private void Start()
        {
            SetProfile(MinimapState.MiniView, new InStateProfile
            {
                MapScale = mapRenderer.localScale,
                RootPositionAnchor = rootUI.anchoredPosition,
                RootUIScale = rootUI.localScale
            });
            SetProfile(MinimapState.Focus, new InStateProfile
            {
                MapScale = new Vector3(4f, 4f, 4f),
                RootPositionAnchor = Vector2.zero,
                RootUIScale = new Vector3(4f, 4f, 4f)
            });
            renderCamera = GetComponent<Camera>();
        }

        public void First()
        {
            DisableFocusView();
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
            rootUI.anchoredPosition = profiles[state].RootPositionAnchor;
            rootUI.localScale = profiles[state].RootUIScale;
            mapRenderer.localScale = profiles[state].MapScale;

            Vector3 pos = renderCamera.WorldToViewportPoint(player.position);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRenderer, pos, null, out Vector2 localPoint);
            Debug.Log(localPoint);
            center.localPosition = localPoint;
        }

        public void SetCamera(Vector3 pos)
        {
            Vector3 cameraPos = pos;
            cameraPos.y = 600f;
            transform.position = cameraPos;
            transform.LookAt(pos);
        }
    }
}