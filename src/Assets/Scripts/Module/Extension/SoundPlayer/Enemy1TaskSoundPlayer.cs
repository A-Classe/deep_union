using Module.Extension.Task;
using Module.Task;
using UnityEngine;

namespace Module.Extension.SoundPlayer
{
    public class Enemy1TaskSoundPlayer : TaskSoundPlayer<Enemy1Task> // ��������p������ <>���͑Ώۂ̃^�X�N�̌^
    {
        //AudioClip��p�ӂ���
        [SerializeField] private AudioClip BombSound;

        protected override void OnStart()
        {
            Task.OnStarted += OnTaskStarted;
            Task.OnCanceled += OnTaskCanceled;
            Task.OnBomb += OnBomb;
        }

        private void OnTaskStarted(BaseTask _)
        {
        }

        private void OnTaskCanceled(BaseTask _)
        {
        }

        private void OnBomb()
        {
            // ������������炷
            //AudioSource.PlayOneShot(BombSound);
        }
    }
}