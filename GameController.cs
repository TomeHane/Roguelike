using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //�K����\������Text�R���|�[�l���g
    [SerializeField]
    Text floorText;
    //�t�F�[�h�A�E�g�E�t�F�[�h�C�����s���X�N���v�g
    [SerializeField]
    UI_blackout blackout;

    //�Ō�̊K
    [SerializeField]
    int lastFloor = 10;
    //���݂̊K
    //SystemManager.cs�ŎQ��
    [System.NonSerialized]
    public int currentFloor = 1;


    private void Start()
    {
        //�K���\����������
        floorText.text = $"B{currentFloor}F";
    }


    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Quit();
        }
    }

    void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
        #endif
    }

    //���̊K�w(�V�[��)�֑J�ڂ���֐�
    public void GoToNextFloor()
    {
        //���݂̊K��+1
        currentFloor++;
        //�K���\����ύX����
        floorText.text = $"B{currentFloor}F";

        //�V�[���̃����[�h
        SceneManager.LoadScene("Maze");

        //�t�F�[�h�C�����������s����
        StartCoroutine(blackout.Fadein());
    }
}
