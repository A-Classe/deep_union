using GameMain.Task;
using Module.Task;
using UnityEngine;

namespace GameMain.SoundPlayer
{
    public class BreakWallTaskSoundPlayer : TaskSoundPlayer<BreakWallTask> // ←これを継承する <>内は対象のタスクの型
    {
        //AudioClipを用意する
        [SerializeField] private AudioClip someSound;

        protected override void OnStart()
        {
            Task.OnStarted += OnTaskStarted;
            Task.OnCanceled += OnTaskCanceled;
            Task.OnCompleted += OnTaskCompleted;
        }

        private void OnTaskStarted(BaseTask _)
        {
            // なんか音ならす
            // AudioSource.PlayOneShot(someSound);
        }

        private void OnTaskCanceled(BaseTask _)
        {
            // なんか音ならす
            // AudioSource.PlayOneShot(someSound);
        }

        private void OnTaskCompleted(BaseTask _)
        {
            // なんか音ならす
            // AudioSource.PlayOneShot(someSound);
        }
    }
}