using System;
using UnityEngine;
using UnityEngine.Video;

namespace Module.Extension.UI
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerController : MonoBehaviour
    {
        private VideoPlayer videoPlayer;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        public void Play(Action callBack, VideoClip clip = null)
        {
            if (clip != null)
            {
                videoPlayer.clip = clip;
            }
            videoPlayer.Play();
            videoPlayer.loopPointReached += _ =>
            {
                callBack?.Invoke();
            };
        }
    }
}