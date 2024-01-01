using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour
{
    public RawImage displayText; // Textコンポーネントを使用する
    [SerializeField] private float time = 3.0f; //デフォルトは3秒
    [SerializeField] private string[] triggerTag;
    [SerializeField] private GameObject[] hideObj;

    private void Start()
    {
        displayText.enabled = false; // 開始時にテキストを非表示にする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerTag.Any(other.CompareTag))
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