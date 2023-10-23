using GameMain.Task;
using Module.Task;
using UnityEngine;

namespace GameMain.SoundPlayer
{
    public class ResourceTaskSoundPlayer : TaskSoundPlayer<ResourceTask> // ��������p������ <>���͑Ώۂ̃^�X�N�̌^
    {
        //AudioClip��p�ӂ���
        [SerializeField] private AudioClip BreakingSound;

        protected override void OnStart()
        {
            Task.OnStarted += OnTaskStarted;
            Task.OnCanceled += OnTaskCanceled;
            Task.OnCompleted += OnTaskCompleted;
        }

        private void OnTaskStarted(BaseTask _)
        {
            // �󂵂Ă��鉹�����[�v�Ŗ炷
            AudioSource.PlayOneShot(BreakingSound);
        }

        private void OnTaskCanceled(BaseTask _)
        {
            // �Ȃ��Ă��鉹���~�߂�
            AudioSource.Stop();
        }

        private void OnTaskCompleted(BaseTask _)
        {

        }
    }
}