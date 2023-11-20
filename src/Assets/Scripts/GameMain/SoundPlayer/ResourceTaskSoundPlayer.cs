using GameMain.Task;
using Module.Task;
using UnityEngine;

namespace GameMain.SoundPlayer
{
    public class ResourceTaskSoundPlayer : TaskSoundPlayer<ResourceTask> // ←これを継承する <>内は対象のタスクの型
    {
        //AudioClipを用意する
        [SerializeField] private AudioClip BreakingSound;

        protected override void OnStart()
        {
            Task.OnStarted += OnTaskStarted;
            Task.OnCanceled += OnTaskCanceled;
            Task.OnCompleted += OnTaskCompleted;
        }

        private void OnTaskStarted(BaseTask _)
        {
            // 壊している音をループで鳴らす
            AudioSource.PlayOneShot(BreakingSound);
        }

        private void OnTaskCanceled(BaseTask _)
        {
            // なっている音を止める
            AudioSource.Stop();
        }

        private void OnTaskCompleted(BaseTask _)
        {
        }
    }
}