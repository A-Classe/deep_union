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
        private Rigidbody leaderRb;

        private void Awake()
        {
            leaderRb = GetComponent<Rigidbody>();

            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
        }

        private void FixedUpdate()
        {
            //リーダーの速度の更新
            UpdateLeaderVelocity();
        }

        private void UpdateLeaderVelocity()
        {
            var input = controlEvent.ReadValue<Vector2>();

            var velocity = leaderRb.velocity;
            velocity.x += input.x * controlSpeed;
            velocity.z += input.y * controlSpeed;

            leaderRb.velocity = velocity;
        }
    }
}