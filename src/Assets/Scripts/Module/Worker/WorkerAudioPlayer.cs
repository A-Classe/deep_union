using UnityEngine;
using Wanna.DebugEx;

namespace Module.Working
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class WorkerAudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                DebugEx.LogError($"{gameObject.name}にAudioSourceがアタッチされていません");
            }
        }

        public void Play()
        {
            //鳴らす
            //audioSource.PlayOneShot();
        }
    }
}