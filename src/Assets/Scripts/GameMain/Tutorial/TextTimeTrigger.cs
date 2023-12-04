using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTimeTrigger : MonoBehaviour
{

    public TextMeshProUGUI displayText; // Text�R���|�[�l���g���g�p����
    [SerializeField] private float displayDelay = 5.0f;
    [SerializeField] private float Time = 3.0f;//�f�t�H���g��3�b
    [SerializeField] private GameObject HideObj = null;


    private void Start()
    {
        displayText.enabled = false; // �J�n���Ƀe�L�X�g���\���ɂ���

        Invoke("ShowText", displayDelay);
    }

    private void ShowText()
    {
        displayText.enabled = true; // �e�L�X�g��\������
        Invoke("HideText", Time);
    }

    private void HideText()
    {
        displayText.enabled = false; // �e�L�X�g���\���ɂ���
        HideObj.SetActive(false);
    }
}
