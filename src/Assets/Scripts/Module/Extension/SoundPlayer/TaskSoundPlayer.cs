using Module.Task;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Extension.SoundPlayer
{
    /// <summary>
    ///     タスクのサウンド機能のベースクラス
    /// </summary>
    /// <typeparam name="T">対象のタスクの型</typeparam>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public abstract class TaskSoundPlayer<T> : MonoBehaviour where T : BaseTask
    {
        protected AudioSource AudioSource;
        protected T Task;

        private void Start()
        {
            Task = GetComponent<T>();
            AudioSource = GetComponent<AudioSource>();

            if (AudioSource == null) DebugEx.LogError($"{gameObject.name}にAudioSourceがアタッチされていません");

            OnStart();
        }

        protected virtual void OnStart()
        {
        }
    }
}