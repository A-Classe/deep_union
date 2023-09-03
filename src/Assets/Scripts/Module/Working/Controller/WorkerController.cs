using System;
using System.Collections.Generic;
using System.Linq;
using Core.Input;
using Core.Utility;
using Module.Working.State;
using UnityEngine;

namespace Module.Working.Controller
{
    /// <summary>
    ///     群体を操作するクラス
    /// </summary>
    public class WorkerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;

        private InputEvent controlEvent;

        private void Awake()
        {
            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
        }

        private void Update()
        {
            //リーダーの速度の更新
            UpdateLeaderVelocity();
        }

        private void UpdateLeaderVelocity()
        {
            var input = controlEvent.ReadValue<Vector2>() ;

            float moveX = input.x * controlSpeed * Time.deltaTime;
            float moveY = input.y * controlSpeed * Time.deltaTime;

            transform.localPosition += new Vector3(moveX, 0f, moveY);
        }
    }
}