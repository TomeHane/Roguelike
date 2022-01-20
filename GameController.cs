using Cinemachine;
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


    //LastFloor�ɂāA�v���C���[�̈ʒu�E��]�𐧌䂷��
    [SerializeField]
    Transform player;
    [SerializeField]
    Transform avator;

    //V�J�����̌��������䂷��
    [SerializeField]
    CinemachineVirtualCamera vCamera;
    //Start()�Ŏ擾
    CinemachineOrbitalTransposer transposer;

    //�X�N���v�g�̃t�B�[���h�l���㏑������
    [SerializeField]
    VCameraController vCameraController;

    //�Q�[���N���A���ɋN��������
    [SerializeField]
    GameObject GameClearController;


    //BGM�EME�E�ꕔSE��炷
    [SerializeField]
    MusicPlayer musicPlayer;

    //�R���[�`�����܂Ƃ߂邽�߂ɕK�v
    enum State
    {
        None,
        ToTitle,
        Quit
    }

    State state = State.None;

    //Update()�ň�x�����������s�����߂̃t���O
    bool isCalledOnce = false;


    private void Start()
    {
        //CinemachineVirtualCamera�R���|�[�l���g�́ABody�������擾
        transposer = vCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        //�K���\����������
        floorText.text = $"B{currentFloor}F";
    }


    private void Update()
    {
        //��x�����s������
        if (!isCalledOnce)
        {
            isCalledOnce = true;

            //��w��BGM��炷
            //���X�N���v�g�̊֐����g������Update()�ɋL�q
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Dungeon01));
        }

        //ESC�L�[�������ꂽ��
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
            //���w��BGM��炷
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Dungeon03));
        }
        //�S�̂̂R���̂P�ɓ��B������
        else if (currentFloor == lastFloor / 3)
        {
            //Directional Light��Ԃ�����
            directionalLight.color = new Color32(150, 80, 80, 255);
            //���w��BGM��炷
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Dungeon02));
        }

        //�V�[���̃��[�h����
        if (currentFloor == lastFloor)
        {
            //Directional Light���Â�����
            directionalLight.color = new Color32(50, 50, 50, 255);
            //�{�X���BGM��炷
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Boss));

            SceneManager.LoadScene("LastFloor");

            //�v���C���[���ړ�������
            player.position = new Vector3(-6.0f, 0f, 26.0f);
            //�v���C���[����]������
            StartCoroutine(FaceFront());
        }
        else
        {
            //�V�[���̃����[�h
            SceneManager.LoadScene("Maze");
        }
        

        //�t�F�[�h�C�����������s����
        StartCoroutine(blackout.Fadein());
    }

    //���ʂ���������
    IEnumerator FaceFront()
    {
        //�����̎������䂪�������Ȃ��悤��1�t���[���҂�
        yield return null;
        //�ύX����̂�localRotation
        avator.localRotation = Quaternion.Euler(0f, 180.0f, 0f);

        //V�J�����̌������ύX����
        transposer.m_Heading.m_Bias = 180f;
        transposer.m_FollowOffset.y = -0.5f;
        //�X�N���v�g�̃t�B�[���h�l���㏑��
        vCameraController.bias = 180f;
        vCameraController.fllowOffsetY = -0.5f;
    }

    //2�b���BGM���Đ�����
    IEnumerator ReserveNextBGM(MusicPlayer.BgmName bgmName, float volume = 0.5f)
    {
        yield return new WaitForSeconds(2.0f);

        musicPlayer.PlayBGM(bgmName, volume);
    }


    //----------�N���A�����𖞂���������s����鏈��(LastFloorManager.cs������)----------
    public void DisplayGameClear()
    {
        //�Q�[���N���A��BGM��炷
        musicPlayer.PlayBGM(MusicPlayer.BgmName.GameClear, 1.0f);

        //�Q�[���N���A��ME��炷
        musicPlayer.PlayME(MusicPlayer.MeName.GameClear, 10.0f, 1.0f);

        //�I�u�W�F�N�g���N������
        GameClearController.SetActive(true);
    }


    //----------�Q�[���I�[�o�[or�Q�[���N���A���ɕ\�������{�^��----------

    //�u�^�C�g���ցv�{�^���������ꂽ�Ƃ��̏���
    public void LoadTitle()
    {
        state = State.ToTitle;
        StartCoroutine(LeaveMaze());
    }

    //�u�Q�[�����I������v�{�^���������ꂽ�Ƃ��̏���
    public void ExitGame()
    {
        state = State.Quit;
        StartCoroutine(LeaveMaze());
    }

    IEnumerator LeaveMaze()
    {
        //�N���b�N����炷
        musicPlayer.PlaySE(MusicPlayer.SeName.Click, 1.0f);

        yield return new WaitForSeconds(1.0f);

        //�^�C�g����ʂ֖߂�
        if (state == State.ToTitle)
        {
            //�J��������؂Ȃ����ƌx�������o��̂ŁA���ǂɕ���ꂽ�T�u�J�������N��
            subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
            subCamera.enabled = true;

            //DontDestroy�I�u�W�F�N�g��S�폜
            dontDestroyManager.DestroyAll();

            //�^�C�g���֖߂�
            SceneManager.LoadScene("Title");
        }

        //�Q�[�����I������
        if (state == State.Quit)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
            #endif
        }
    }
}
