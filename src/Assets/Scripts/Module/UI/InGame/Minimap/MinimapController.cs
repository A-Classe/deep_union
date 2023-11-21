using System;
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
            moveState = ControlState.Idle;
        }

        private Vector3 lastMovePosition = Vector3.zero;
        private void FixedUpdate()
        {
            switch (moveState)
            {
                case ControlState.MoveToFocus:
                    UpdateInMoveFocus();
                    break;
                case ControlState.MoveToMiniView:
                    UpdateInMoveMinimap();
                    break;
                case ControlState.Idle:
                    UpdateInCamera();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateInCamera()
        {
            if (Vector3.Distance(lastMovePosition, mapRenderer.position) > 1f)
            {
                Vector3 pos = mapDefaultPos;
                Vector3 viewPortValue = renderCamera.WorldToViewportPoint(player.transform.position);
                pos.y -= viewPortValue.y * (mapRenderer.rect.height * mapRenderer.localScale.y);
                Vector3 mapPos = mapRenderer.position;
                mapPos.y = pos.y;
                mapRenderer.position = mapPos;
                lastMovePosition = player.transform.position;
            }
        }

        private void UpdateInMoveFocus()
        {
            rootUI.anchoredPosition = Vector2.Lerp(rootUI.anchoredPosition, profiles[state].RootPositionAnchor, Time.deltaTime);
            rootUI.localScale = Vector3.Lerp(rootUI.localScale, profiles[state].RootUIScale, Time.deltaTime);
           // mapRenderer.localScale = Vector3.Lerp(mapRenderer.localScale, profiles[state].MapScale, Time.deltaTime);
            if (Vector2.Distance(rootUI.anchoredPosition, profiles[state].RootPositionAnchor) < 0.1f)
            {
                UpdateInProfile();
                moveState = ControlState.Idle;
            }
        }

        private void UpdateInMoveMinimap()
        {
            rootUI.anchoredPosition = Vector2.Lerp(rootUI.anchoredPosition, profiles[state].RootPositionAnchor, Time.deltaTime);
            rootUI.localScale = Vector3.Lerp(rootUI.localScale, profiles[state].RootUIScale, Time.deltaTime);
          //  mapRenderer.localScale = Vector3.Lerp(mapRenderer.localScale, profiles[state].MapScale, Time.deltaTime);
            if (Vector2.Distance(rootUI.anchoredPosition, profiles[state].RootPositionAnchor) < 0.1f)
            {
                UpdateInProfile();
                moveState = ControlState.Idle;
            }
        }

        internal void SetProfile(MinimapState key, InStateProfile profile)
        {
            profiles[key] = profile;
            
        }

        public void EnableFocusView()
        {
            moveState = ControlState.MoveToFocus;
            state = MinimapState.Focus;
            UpdateInProfile();
        }

        public void DisableFocusView()
        {
            moveState = ControlState.MoveToMiniView;
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