using UnityEngine;
using UnityEngine.UI;

public class ImageTrigger : MonoBehaviour
{
    public RawImage rawImage; // RawImage�R���|�[�l���g���g�p����
    [SerializeField] private float Time = 3.0f;//�f�t�H���g��3�b

    private void Start()
    {
        rawImage.enabled = false; // �J�n���ɉ摜���\���ɂ���
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rawImage.enabled = true; // �摜��\������
            Invoke("HideImage", Time); // Time�b���HideImage���\�b�h���Ă�
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideImage(); // �v���C���[���G���A����o���炷���ɔ�\���ɂ���
        }
    }

    private void HideImage()
    {
        rawImage.enabled = false; // �摜���\���ɂ���
    }
}