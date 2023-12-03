using UnityEngine;
using UnityEngine.UI;

public class ImageTimeTrigger : MonoBehaviour
{
    [SerializeField] private RawImage imageToDisplay;
    [SerializeField] private float displayDelay = 5f;// 表示までの遅延時間（秒）
    [SerializeField] private float time = 3.0f;
    [SerializeField] private GameObject hideObj = null;

    private void Start()
    {
        // ゲーム開始時に非表示にする
        imageToDisplay.enabled = false;

        // 一定時間が経過したら画像を表示する
        Invoke(nameof(DisplayImage), displayDelay);      
    }

    private void DisplayImage()
    {
        // 画像を表示する
        imageToDisplay.enabled = true;
        Invoke(nameof(HideImage), time); // Time秒後にHideImageメソッドを呼ぶ
    }
    private void HideImage()
    {
        imageToDisplay.enabled = false; // 画像を非表示にする
        hideObj.SetActive(false);
    }   
}