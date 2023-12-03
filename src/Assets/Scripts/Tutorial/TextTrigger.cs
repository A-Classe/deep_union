using UnityEngine;
using TMPro;

public class TextTrigger : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Textコンポーネントを使用する
    [SerializeField] private float time = 3.0f;//デフォルトは3秒
    [SerializeField] private string detectTag = "Player";
    [SerializeField] private GameObject hideObj = null;


    private void Start()
    {
        displayText.enabled = false; // 開始時にテキストを非表示にする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectTag))
        {
            ShowText(); // プレイヤーがエリアに入っており、テキストがまだ表示されていない場合は表示する
            Invoke(nameof(HideText), time); // Time秒後に非表示にする
        }
    }

    private void ShowText()
    {
        displayText.enabled = true; // テキストを表示する
    }

    private void HideText()
    {
        displayText.enabled = false; // テキストを非表示にする
        hideObj.SetActive(false);
    }
}