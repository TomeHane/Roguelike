using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //階数を表示するTextコンポーネント
    [SerializeField]
    Text floorText;
    //フェードアウト・フェードインを行うスクリプト
    [SerializeField]
    UI_blackout blackout;

    //最後の階
    [SerializeField]
    int lastFloor = 10;
    //現在の階
    //SystemManager.csで参照
    [System.NonSerialized]
    public int currentFloor = 1;


    private void Start()
    {
        //階数表示を初期化
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

    //次の階層(シーン)へ遷移する関数
    public void GoToNextFloor()
    {
        //現在の階に+1
        currentFloor++;
        //階数表示を変更する
        floorText.text = $"B{currentFloor}F";

        //シーンのリロード
        SceneManager.LoadScene("Maze");

        //フェードイン処理を実行する
        StartCoroutine(blackout.Fadein());
    }
}
