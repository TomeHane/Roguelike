using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    MusicPlayer musicPlayer;

    //�R���[�`�����܂Ƃ߂邽�߂ɕK�v
    enum State
    {
        None,
        Start,
        Quit
    }

    State state = State.None;

    private void Start()
    {
        //BGM���w�肵�Ė炷
        musicPlayer.PlayBGM(MusicPlayer.BgmName.Title);
    }

    public void StartGame()
    {
        state = State.Start;
        StartCoroutine(LeaveTitle());
    }

    public void QuitGame()
    {
        state = State.Quit;
        StartCoroutine(LeaveTitle());
    }

    //�X�e�[�^�X�ɂ���ď����𕪂���
    IEnumerator LeaveTitle()
    {
        //�N���b�N����炷
        musicPlayer.PlaySE(MusicPlayer.SeName.Click, 1.0f);

        yield return new WaitForSeconds(1.0f);

        //�Q�[�����J�n����
        if (state == State.Start)
        {
            SceneManager.LoadScene("CreateObjects");
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
