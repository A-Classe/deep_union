using UnityEngine;
using UnityEngine.Audio;

namespace Module.GameSetting
{
    public class AudioMixerController
    {
        private readonly AudioMixer mixer;

        public AudioMixerController(
            AudioMixer mixer
        )
        {
            this.mixer = mixer;
        }

        private const string BGMVolume = "BGM";
        private const string SeVolume = "SE";
        private const string MasterVolume = "Master";

        public void SetBGMVolume(float value)
        {
            mixer.SetFloat(BGMVolume, GetAudioRate(value));
        }

        // ReSharper disable once InconsistentNaming
        public void SetSEVolume(float value)
        {
            mixer.SetFloat(SeVolume, GetAudioRate(value));
        }

        public void SetMasterVolume(float value)
        {
            mixer.SetFloat(MasterVolume, GetAudioRate(value));
        }

        private float GetAudioRate(float value)
        {
            return Mathf.Clamp(20f * Mathf.Log10(value), -80f, 0f);
        }
    }
}