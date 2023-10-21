using GameMain.Task;
using Module.Task;
using UnityEngine;

namespace GameMain.SoundPlayer
{
    public class Enemy1TaskSoundPlayer : TaskSoundPlayer<Enemy1Task> // ←これを継承する <>内は対象のタスクの型
    {
        //AudioClipを用意する
        [SerializeField] private AudioClip BombSound;

        protected override void OnStart()
        {
            Task.OnStarted += OnTaskStarted;
            Task.OnCanceled += OnTaskCanceled;
            Task.OnCompleted += OnTaskCompleted;
        }

        private void OnTaskStarted(BaseTask _)
        {
            
        }

        private void OnTaskCanceled(BaseTask _)
        {
            
        }

        private void OnTaskCompleted(BaseTask _)
        {
            // 爆発した音を鳴らす
            AudioSource.PlayOneShot(BombSound);
        }
    }
}