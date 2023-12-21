using Module.Extension.Task;
using Module.Task;
using UnityEngine;

namespace Module.Extension.SoundPlayer
{
    public class IncreaseTaskSoundPlayer : TaskSoundPlayer<IncreaseWorkerTask> // ��������p������ <>���͑Ώۂ̃^�X�N�̌^
    {
        //AudioClip��p�ӂ���
        [SerializeField] private AudioClip CompleteSound;

        protected override void OnStart()
        {
            Task.OnStarted += OnTaskStarted;
            Task.OnCanceled += OnTaskCanceled;
            Task.OnCompleted += OnTaskCompleted;
        }

        private void OnTaskStarted(BaseTask _) { }

        private void OnTaskCanceled(BaseTask _) { }

        private void OnTaskCompleted(BaseTask _)
        {
            // ������������炷
            AudioSource.PlayOneShot(CompleteSound);
        }
    }
}