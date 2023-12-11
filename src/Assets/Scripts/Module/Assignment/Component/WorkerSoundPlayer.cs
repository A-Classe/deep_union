using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wanna.DebugEx;


namespace Module.Working
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class WorkerSoundPlayer : MonoBehaviour
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
