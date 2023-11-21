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
        private ControlState moveState;
        private MinimapState state;
        
        private Camera renderCamera;

        private Vector3 mapDefaultPos;

        private Dictionary<MinimapState, InStateProfile> profiles = new();


        private void Start()
        {
            SetProfile(MinimapState.MiniView, new InStateProfile
            {
                MapScale = new Vector3(6f, 6f, 6f),
                RootPositionAnchor = rootUI.anchoredPosition,
                RootUIScale = rootUI.localScale
            });
            SetProfile(MinimapState.Focus, new InStateProfile
            {
                MapScale = new Vector3(3f, 3f, 3f),
                RootPositionAnchor = Vector2.zero,
                RootUIScale = new Vector3(4f, 4f, 4f)
            });
            renderCamera = GetComponent<Camera>();
        }

        public void First()
        {
            DisableFocusView();
            // mapのbottomをrootのbottomに合わせる
            var pos = rootUI.position;
            pos.y -= (rootUI.rect.height / 2f) * rootUI.localScale.y;
            pos.x = mapRenderer.position.x;
            pos.y += (mapRenderer.rect.height / 2f) * mapRenderer.localScale.y;
            mapRenderer.position = pos;


            mapDefaultPos = pos;
        }

        private Vector3 lastMovePosition = Vector3.zero;
        private void Update()
        {
            Vector3 pos = mapDefaultPos;
            Vector3 viewPortValue = renderCamera.WorldToViewportPoint(player.transform.position);
            pos.y -= viewPortValue.y * (mapRenderer.rect.height * mapRenderer.localScale.y);
            Vector3 mapPos = mapRenderer.position;
            mapPos.y = pos.y;
            mapRenderer.position = mapPos;
            lastMovePosition = player.transform.position;
            if (Vector3.Distance(lastMovePosition, mapRenderer.position) > 1f)
            {
            }
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
            
            // float referenceBottom = rootUI.anchoredPosition.y - rootUI.rect.height / 2;
            //
            // float targetY = referenceBottom + mapRenderer.rect.height / 2;
            // mapRenderer.anchoredPosition = new Vector2(mapRenderer.anchoredPosition.x, targetY);
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