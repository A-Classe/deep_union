using Module.Extension.Task;
using Module.Task;
using UnityEngine;

namespace Module.Extension.SoundPlayer
{
    public class HealTaskSoundPlayer : TaskSoundPlayer<HealTask> // ��������p������ <>���͑Ώۂ̃^�X�N�̌^
    {
        //AudioClip��p�ӂ���
        [SerializeField] private AudioClip HealSound;

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
            // �񕜂�������炷
            AudioSource.PlayOneShot(HealSound);
        }
    }
}