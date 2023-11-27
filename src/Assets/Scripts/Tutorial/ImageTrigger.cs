using UnityEngine;
using UnityEngine.UI;

public class ImageTrigger : MonoBehaviour
{
    public RawImage rawImage; // RawImageコンポーネントを使用する
    [SerializeField] private float Time = 3.0f;//デフォルトは3秒

    private void Start()
    {
        rawImage.enabled = false; // 開始時に画像を非表示にする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rawImage.enabled = true; // 画像を表示する
            Invoke("HideImage", Time); // Time秒後にHideImageメソッドを呼ぶ
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideImage(); // プレイヤーがエリアから出たらすぐに非表示にする
        }
    }

    private void HideImage()
    {
        rawImage.enabled = false; // 画像を非表示にする
    }
}