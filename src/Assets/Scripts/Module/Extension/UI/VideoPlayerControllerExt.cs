using UnityEngine;
using UnityEngine.Video;

namespace Module.Extension.UI
{
    
    /// <summary>
    /// Resultの背景組み込み用
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerControllerExt: MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        [SerializeField] private VideoClip[] videoClips;
        private int currentVideoIndex = 0;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        public void Play()
        {
            PlayNextVideo();
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            // 最後の動画以外の場合に次の動画を再生
            if (currentVideoIndex < videoClips.Length)
            {
                PlayNextVideo();
            }
        }

        private void PlayNextVideo()
        {
            if (currentVideoIndex >= videoClips.Length) return;

            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Play();

            // 最後の動画の場合、ループを有効にする
            if (currentVideoIndex == videoClips.Length - 1)
            {
                videoPlayer.isLooping = true;
            }

            currentVideoIndex++;
        }
    }
}