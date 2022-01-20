using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    MusicPlayer musicPlayer;

    //コルーチンをまとめるために必要
    enum State
    {
        None,
        Start,
        Quit
    }

    State state = State.None;

    private void Start()
    {
        //BGMを指定して鳴らす
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

    //ステータスによって処理を分ける
    IEnumerator LeaveTitle()
    {
        //クリック音を鳴らす
        musicPlayer.PlaySE(MusicPlayer.SeName.Click, 1.0f);

        yield return new WaitForSeconds(1.0f);

        //ゲームを開始する
        if (state == State.Start)
        {
            SceneManager.LoadScene("CreateObjects");
        }

        //ゲームを終了する
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
