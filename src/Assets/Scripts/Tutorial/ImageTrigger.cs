using UnityEngine;
using UnityEngine.UI;

public class ImageTrigger : MonoBehaviour
{
    public RawImage rawImage; // RawImage�R���|�[�l���g���g�p����
    [SerializeField] private float Time = 3.0f;//�f�t�H���g��3�b
    [SerializeField] private string Tag = "Player";
    [SerializeField] private GameObject[] HideObj;

    private void Start()
    {
        rawImage.enabled = false; // �J�n���ɉ摜���\���ɂ���
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            rawImage.enabled = true; // �摜��\������
            Invoke("HideImage", Time); // Time�b���HideImage���\�b�h���Ă�
        }
    }


    private void HideImage()
    {
        rawImage.enabled = false; // �摜���\���ɂ���
        foreach (GameObject obj in HideObj)
        {
            Destroy(obj);
        }
    }
}