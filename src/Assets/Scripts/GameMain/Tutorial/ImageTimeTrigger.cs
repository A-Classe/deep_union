using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTimeTrigger : MonoBehaviour
{
    [SerializeField] private RawImage imageToDisplay;
    [SerializeField] private float displayDelay = 5f;// 表示までの遅延時間（秒）
    [SerializeField] private float Time = 3.0f;
    [SerializeField] private GameObject HideObj = null;

    private void Start()
    {
        // ゲーム開始時に非表示にする
        imageToDisplay.enabled = false;

        // 一定時間が経過したら画像を表示する
        Invoke("DisplayImage", displayDelay);      
    }

    private void DisplayImage()
    {
        // 画像を表示する
        imageToDisplay.enabled = true;
        Invoke("HideImage", Time); // Time秒後にHideImageメソッドを呼ぶ
    }
    private void HideImage()
    {
        imageToDisplay.enabled = false; // 画像を非表示にする
        HideObj.SetActive(false);
    }   
}