using System;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using UnityEngine;

namespace GameMain.Task
{
    /// <summary>
    /// - タスク終了後にgameObjectを削除する
    /// - 完了にしなければplayerは前に進めない
    /// </summary>
    public class DeletedObjectTask : BaseTask
    {
        /// <summary>
        /// タスク終了後にhitしたplayerのstateを変更する
        /// </summary>
        private Action onComplete;
        protected override void OnComplete()
        {
            base.OnComplete();
            onComplete?.Invoke();
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 子objectのwaitPointからタスク完了までplayerをその場に止める
        /// </summary>
        /// <param name="other">hitしたgameObject</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            var obj = other.gameObject;

            var controller = obj.GetComponent<PlayerController>();
            if (controller == null) return;
            
            controller.SetState(PlayerState.Wait);
            onComplete = () => controller.SetState(PlayerState.Go);
        }
        
    }
}
