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
    //DirectionalLightのLightコンポーネント
    [SerializeField]
    Light directionalLight;

    //最後の階
    [SerializeField]
    int lastFloor = 10;
    //現在の階
    //SystemManager.csで参照
    [System.NonSerialized]
    public int currentFloor = 1;


    //DDOLオブジェクトを全削除するために
    [SerializeField]
    DontDestroyManager dontDestroyManager;
    //カメラをゼロにしないように
    Camera subCamera;


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

        
        //全体の３分の２に到達したら
        if (currentFloor == 2 * lastFloor / 3)
        {
            //Directional Lightを青くする
            directionalLight.color = new Color32(30, 30, 200, 255);
        }
        //全体の３分の１に到達したら
        else if (currentFloor == lastFloor / 3)
        {
            //Directional Lightを赤くする
            directionalLight.color = new Color32(150, 80, 80, 255);
        }

        if (currentFloor == lastFloor)
        {
            //Directional Lightを白くする
            directionalLight.color = new Color32(200, 200, 200, 255);
            
            SceneManager.LoadScene("LastFloor");
        }
        else
        {
            //シーンのリロード
            SceneManager.LoadScene("Maze");
        }
        

        //フェードイン処理を実行する
        StartCoroutine(blackout.Fadein());
    }


    //----------ゲームオーバーorゲームクリア時に表示されるボタン----------

    //「タイトルへ」ボタンが押されたときの処理
    public void LoadTitle()
    {
        //カメラを一切なくすと警告文が出るので、黒壁に覆われたサブカメラを起動
        subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
        subCamera.enabled = true;

        //DontDestroyオブジェクトを全削除
        dontDestroyManager.DestroyAll();

        //タイトルへ戻る
        SceneManager.LoadScene("Title");
    }

    //「ゲームを終了する」ボタンが押されたときの処理
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
        UnityEngine.Application.Quit();
        #endif
    }
}
