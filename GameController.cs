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
    //DirectionalLight��Light�R���|�[�l���g
    [SerializeField]
    Light directionalLight;

    //�Ō�̊K
    [SerializeField]
    int lastFloor = 10;
    //���݂̊K
    //SystemManager.cs�ŎQ��
    [System.NonSerialized]
    public int currentFloor = 1;


    //DDOL�I�u�W�F�N�g��S�폜���邽�߂�
    [SerializeField]
    DontDestroyManager dontDestroyManager;
    //�J�������[���ɂ��Ȃ��悤��
    Camera subCamera;


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

        
        //�S�̂̂R���̂Q�ɓ��B������
        if (currentFloor == 2 * lastFloor / 3)
        {
            //Directional Light�������
            directionalLight.color = new Color32(30, 30, 200, 255);
        }
        //�S�̂̂R���̂P�ɓ��B������
        else if (currentFloor == lastFloor / 3)
        {
            //Directional Light��Ԃ�����
            directionalLight.color = new Color32(150, 80, 80, 255);
        }

        if (currentFloor == lastFloor)
        {
            //Directional Light�𔒂�����
            directionalLight.color = new Color32(200, 200, 200, 255);
            
            SceneManager.LoadScene("LastFloor");
        }
        else
        {
            //�V�[���̃����[�h
            SceneManager.LoadScene("Maze");
        }
        

        //�t�F�[�h�C�����������s����
        StartCoroutine(blackout.Fadein());
    }


    //----------�Q�[���I�[�o�[or�Q�[���N���A���ɕ\�������{�^��----------

    //�u�^�C�g���ցv�{�^���������ꂽ�Ƃ��̏���
    public void LoadTitle()
    {
        //�J��������؂Ȃ����ƌx�������o��̂ŁA���ǂɕ���ꂽ�T�u�J�������N��
        subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
        subCamera.enabled = true;

        //DontDestroy�I�u�W�F�N�g��S�폜
        dontDestroyManager.DestroyAll();

        //�^�C�g���֖߂�
        SceneManager.LoadScene("Title");
    }

    //�u�Q�[�����I������v�{�^���������ꂽ�Ƃ��̏���
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
        #endif
    }
}
