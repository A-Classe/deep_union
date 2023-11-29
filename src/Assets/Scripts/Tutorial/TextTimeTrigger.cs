using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTimeTrigger : MonoBehaviour
{

    public TextMeshProUGUI displayText; // Textコンポーネントを使用する
    [SerializeField] private float displayDelay = 5.0f;
    [SerializeField] private float Time = 3.0f;//デフォルトは3秒
    [SerializeField] private GameObject HideObj = null;


    private void Start()
    {
        displayText.enabled = false; // 開始時にテキストを非表示にする

        Invoke("ShowText", displayDelay);
    }

    private void ShowText()
    {
        displayText.enabled = true; // テキストを表示する
        Invoke("HideText", Time);
    }

    private void HideText()
    {
        displayText.enabled = false; // テキストを非表示にする
        HideObj.SetActive(false);
    }
}
