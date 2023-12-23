using System;
using System.Collections.Generic;
using Module.Task;
using UnityEngine;
using UnityEngine.VFX;
using Wanna.DebugEx;

namespace Module.Extension.SoundPlayer
{
    /// <summary>
    ///     タスクのサウンド機能のベースクラス
    /// </summary>
    /// <typeparam name="T">対象のタスクの型</typeparam>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public abstract class TaskEventTrigger<T> : MonoBehaviour where T : BaseTask
    {
        [SerializeField] private List<TaskProgressEvent> progressEvents;
        protected AudioSource AudioSource;
        protected T Task;

        private List<TaskProgressEvent>.Enumerator currentEvent;

        private void Start()
        {
            Task = GetComponent<T>();
            AudioSource = GetComponent<AudioSource>();

            if (AudioSource == null)
            {
                DebugEx.LogError($"{gameObject.name}にAudioSourceがアタッチされていません");
            }

            if (progressEvents.Count > 0)
            {
                currentEvent = progressEvents.GetEnumerator();
                currentEvent.MoveNext();
                Task.OnProgressChanged += OnTaskProgressChanged;
            }

            OnStart();
        }

        /// <summary>
        /// タスクの進捗が変更された時にイベントを発生させます。
        /// </summary>
        /// <param name="progress">タスクの進捗</param>
        private void OnTaskProgressChanged(float progress)
        {
            TaskProgressEvent progressEvent = currentEvent.Current;

            //タスクの進捗がイベントに満たしていなかったら終了
            if (progressEvent == null || progressEvent.Progress > progress)
            {
                return;
            }

            if (progressEvent.AudioClip != null)
            {
                AudioSource.PlayOneShot(progressEvent.AudioClip);
            }

            if (progressEvent.VisualEffect != null)
            {
                progressEvent.VisualEffect.Play();
            }

            currentEvent.MoveNext();
            OnTaskProgressChanged(progress);
        }

        protected virtual void OnStart() { }
    }

    [Serializable]
    internal class TaskProgressEvent
    {
        [SerializeField] [Range(0f, 1f)] private float progress;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private VisualEffect visualEffect;

        public float Progress => progress;
        public AudioClip AudioClip => audioClip;
        public VisualEffect VisualEffect => visualEffect;
    }
}