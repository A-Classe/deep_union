using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTrigger : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Text�R���|�[�l���g���g�p����
    [SerializeField] private float Time = 3.0f;//�f�t�H���g��3�b
    [SerializeField] private string Tag = "Player";
    [SerializeField] private GameObject HideObj = null;


    private void Start()
    {
        displayText.enabled = false; // �J�n���Ƀe�L�X�g���\���ɂ���
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            ShowText(); // �v���C���[���G���A�ɓ����Ă���A�e�L�X�g���܂��\������Ă��Ȃ��ꍇ�͕\������
            Invoke("HideText", Time); // Time�b��ɔ�\���ɂ���
        }
    }

    private void ShowText()
    {
        displayText.enabled = true; // �e�L�X�g��\������
    }

    private void HideText()
    {
        displayText.enabled = false; // �e�L�X�g���\���ɂ���
        HideObj.SetActive(false);
    }
}