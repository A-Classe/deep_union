using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTimeTrigger : MonoBehaviour
{
    [SerializeField] private RawImage imageToDisplay;
    [SerializeField] private float displayDelay = 5f;// �\���܂ł̒x�����ԁi�b�j
    [SerializeField] private float Time = 3.0f;
    [SerializeField] private GameObject HideObj = null;

    private void Start()
    {
        // �Q�[���J�n���ɔ�\���ɂ���
        imageToDisplay.enabled = false;

        // ��莞�Ԃ��o�߂�����摜��\������
        Invoke("DisplayImage", displayDelay);      
    }

    private void DisplayImage()
    {
        // �摜��\������
        imageToDisplay.enabled = true;
        Invoke("HideImage", Time); // Time�b���HideImage���\�b�h���Ă�
    }
    private void HideImage()
    {
        imageToDisplay.enabled = false; // �摜���\���ɂ���
        HideObj.SetActive(false);
    }   
}