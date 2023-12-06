using System;
using System.Threading.Tasks;
using Core.Input;
using GameMain.Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class StartImageTrigger : MonoBehaviour, ITutorialTrigger
{
    public RawImage rawImage; // RawImageコンポーネントを使用する
    [SerializeField] private float Time = 3.0f; //デフォルトは3秒
    [SerializeField] private string Tag = "Player";
    [SerializeField] private GameObject[] HideObj;

    public event Action OnShowText;
    public event Action OnHideText;

    private InputEvent confirmEvent;

    private void Start()
    {
        rawImage.enabled = false; // 開始時に画像を非表示にする

        confirmEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.UI.Confirm);

        confirmEvent.Started += _ =>
        {
            if (rawImage.enabled)
            {
                HideImage();
            }
        };
    }

    private async void OnTriggerEnter(Collider other)
    {
        await Task.Delay(500);

        if (other.CompareTag(Tag))
        {
            rawImage.enabled = true; // 画像を表示する
            OnShowText?.Invoke();
        }
    }

    private void HideImage()
    {
        rawImage.enabled = false; // 画像を非表示にする
        OnHideText?.Invoke();
        foreach (GameObject obj in HideObj)
        {
            Destroy(obj);
        }

        confirmEvent.Clear();
    }
}