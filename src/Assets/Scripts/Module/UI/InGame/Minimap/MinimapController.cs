using System.Collections.Generic;
using Core.Input;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

namespace Module.UI.InGame.Minimap
{
    [RequireComponent(typeof(Camera))]
    public class MinimapController: MonoBehaviour
    {
        // private enum ControlState
        // {
        //     MoveToMiniView,
        //     MoveToFocus,
        //     Idle
        // }

        [SerializeField] private RectTransform rootUI;

        [SerializeField] private RectTransform mapRenderer;
        
        [SerializeField] private RectTransform leaderRenderer;
        
        [SerializeField] private Transform leader;

        [SerializeField] private RectTransform playerRenderer;

        [SerializeField] private Transform player;
        // private ControlState moveState;
        private MinimapState state;
        
        private Camera renderCamera;

        private Vector3 mapDefaultPos;

        private Dictionary<MinimapState, InStateProfile> profiles = new();


        private float inputPositionY;

        private InputEvent moveEvent;
        

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
                RootUIScale = new Vector3(5f, 5f, 5f)
            });
            renderCamera = GetComponent<Camera>();
            
            moveEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Title.Move);
        }

        public void First()
        {
            DisableFocusView();
            // mapのbottomをrootのbottomに合わせる
        }

        private Vector3 lastMovePosition = Vector3.zero;
        private void Update()
        {
            UpdateInCamera();

            Vector2 dir = moveEvent.ReadValue<Vector2>();
            InputDirection(dir);
        }

        private void UpdateInCamera()
        {

            Vector3 viewPortValue = renderCamera.WorldToViewportPoint(leader.transform.position);

            Vector3 baseMinWorld = rootUI.position + new Vector3(
                (-rootUI.rect.width) / 2 * rootUI.localScale.x,
                (-rootUI.rect.height) / 2 * rootUI.localScale.y, 
                0
            );

            Vector3 targetPosition = baseMinWorld + new Vector3(
                (rootUI.rect.width) * viewPortValue.x * rootUI.localScale.x,
                (mapRenderer.rect.height / 2 -
                 mapRenderer.rect.height * (viewPortValue.y + inputPositionY)) * rootUI.localScale.y * mapRenderer.localScale.y,
                0
            );

            // ターゲットUIの位置を設定
            mapRenderer.position = targetPosition;
            lastMovePosition = leader.transform.position;
            AlignToBaseUIWithScale(mapRenderer, leaderRenderer, viewPortValue);
                
            viewPortValue = renderCamera.WorldToViewportPoint(player.transform.position);
            AlignToBaseUIWithScale(mapRenderer, playerRenderer, viewPortValue);
        }


        internal void SetProfile(MinimapState key, InStateProfile profile)
        {
            profiles[key] = profile;
        }

        public void EnableFocusView()
        {
         //   moveState = ControlState.MoveToFocus;
            state = MinimapState.Focus;
            UpdateInProfile();
        }

        public void DisableFocusView()
        {
        //    moveState = ControlState.MoveToMiniView;
            state = MinimapState.MiniView;
            UpdateInProfile();
        }

        internal MinimapState GetState() => state;

        private void UpdateInProfile()
        {
            rootUI.anchoredPosition = profiles[state].RootPositionAnchor;
            rootUI.localScale = profiles[state].RootUIScale;
            mapRenderer.localScale = profiles[state].MapScale;
            inputPositionY = -0.03f;
            // float referenceBottom = rootUI.anchoredPosition.y - rootUI.rect.height / 2;
            //
            // float targetY = referenceBottom + mapRenderer.rect.height / 2;
            // mapRenderer.anchoredPosition = new Vector2(mapRenderer.anchoredPosition.x, targetY);
        }

        private void InputDirection(Vector2 dir)
        {     
            if (state == MinimapState.Focus)
            {
                inputPositionY += dir.y / 200f;
            }
        }

        public void SetCamera(Vector3 pos)
        {
            Vector3 cameraPos = pos;
            cameraPos.y = 600f;
            transform.position = cameraPos;
            transform.LookAt(pos);
        }
        
        private void AlignToBottom(RectTransform baseUI, RectTransform targetUI)
        {
            // baseUIのワールド座標を取得
            Vector3 basePosition = baseUI.position;

            // baseUIの高さの半分を計算
            float baseHeightHalf = baseUI.rect.height / 2;

            // targetUIの高さの半分を計算
            float targetHeightHalf = targetUI.rect.height / 2;

            // targetUIの新しい位置を設定（baseUIの下部に合わせる）
            targetUI.position = new Vector3(targetUI.position.x, basePosition.y - baseHeightHalf - targetHeightHalf, targetUI.position.z);
        }

        private void AlignToBaseUIWithScale(RectTransform baseUI, RectTransform targetUI, Vector2 viewPortValue)
        {
            Vector3 baseMinWorld = rootUI.position + new Vector3(
                (-rootUI.rect.width) / 2 * rootUI.localScale.x,
                (-rootUI.rect.height) / 2 * rootUI.localScale.y, 
                0
            );

            Vector3 targetPosition = baseMinWorld + new Vector3(
                (rootUI.rect.width) * viewPortValue.x * rootUI.localScale.x,
                (baseUI.rect.height * (-inputPositionY) * mapRenderer.localScale.y + targetUI.rect.height / 2 - viewPortValue.y * baseUI.rect.height * mapRenderer.localScale.y) * rootUI.localScale.y,
                0
            );
            
            targetUI.position = targetPosition;
        }
    }
}