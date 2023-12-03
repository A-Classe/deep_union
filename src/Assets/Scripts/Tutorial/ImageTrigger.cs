using UnityEngine;
using UnityEngine.UI;

public class ImageTrigger : MonoBehaviour
{
    public RawImage rawImage; // RawImageコンポーネントを使用する
    [SerializeField] private float time = 3.0f;//デフォルトは3秒
    [SerializeField] private string detectTag = "Player";
    [SerializeField] private GameObject[] hideObj;

    private void Start()
    {
        rawImage.enabled = false; // 開始時に画像を非表示にする
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectTag))
        {
            rawImage.enabled = true; // 画像を表示する
            Invoke(nameof(HideImage), time); // Time秒後にHideImageメソッドを呼ぶ
        }
    }


    private void HideImage()
    {
        rawImage.enabled = false; // 画像を非表示にする
        foreach (GameObject obj in hideObj)
        {
            Destroy(obj);
        }
    }
}