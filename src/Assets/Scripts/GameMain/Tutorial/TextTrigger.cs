using System;
using Core.Input;
using Cysharp.Threading.Tasks;
using GameMain.Tutorial;
using UnityEngine;
using UnityEngine.UI;

using Wanna.DebugEx;

public class TextTrigger : MonoBehaviour
{
    public RawImage displayText; // Textコンポーネントを使用する
    [SerializeField] private float time = 3.0f; //デフォルトは3秒
    [SerializeField] private string triggerTag = "Player";
    [SerializeField] private GameObject[] hideObj = null;

    private void Start()
    {
        displayText.enabled = false; // 開始時にテキストを非表示にする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            ShowText(); // プレイヤーがエリアに入っており、テキストがまだ表示されていない場合は表示する
            Invoke(nameof(HideText), time);
        }
    }

    private void ShowText()
    {
        displayText.enabled = true; // テキストを表示する
    }

    private void HideText()
    {
        displayText.enabled = false; // テキストを非表示にする
        foreach (GameObject obj in hideObj)
        {
            Destroy(obj);
        }
    }
}