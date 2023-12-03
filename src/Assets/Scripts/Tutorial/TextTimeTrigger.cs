using TMPro;
using UnityEngine;

public class TextTimeTrigger : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Textコンポーネントを使用する
    [SerializeField] private float displayDelay = 5.0f;
    [SerializeField] private float time = 3.0f;//デフォルトは3秒
    [SerializeField] private GameObject hideObj = null;


    private void Start()
    {
        displayText.enabled = false; // 開始時にテキストを非表示にする

        Invoke(nameof(ShowText), displayDelay);
    }

    private void ShowText()
    {
        displayText.enabled = true; // テキストを表示する
        Invoke(nameof(HideText), time);
    }

    private void HideText()
    {
        displayText.enabled = false; // テキストを非表示にする
        hideObj.SetActive(false);
    }
}
