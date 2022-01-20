using Cinemachine;
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


    //LastFloorにて、プレイヤーの位置・回転を制御する
    [SerializeField]
    Transform player;
    [SerializeField]
    Transform avator;

    //Vカメラの向きも制御する
    [SerializeField]
    CinemachineVirtualCamera vCamera;
    //Start()で取得
    CinemachineOrbitalTransposer transposer;

    //スクリプトのフィールド値も上書きする
    [SerializeField]
    VCameraController vCameraController;

    //ゲームクリア時に起動させる
    [SerializeField]
    GameObject GameClearController;


    //BGM・ME・一部SEを鳴らす
    [SerializeField]
    MusicPlayer musicPlayer;

    //コルーチンをまとめるために必要
    enum State
    {
        None,
        ToTitle,
        Quit
    }

    State state = State.None;

    //Update()で一度だけ処理を行うためのフラグ
    bool isCalledOnce = false;


    private void Start()
    {
        //CinemachineVirtualCameraコンポーネントの、Body部分を取得
        transposer = vCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        //階数表示を初期化
        floorText.text = $"B{currentFloor}F";
    }


    private void Update()
    {
        //一度だけ行う処理
        if (!isCalledOnce)
        {
            isCalledOnce = true;

            //上層のBGMを鳴らす
            //他スクリプトの関数を使うためUpdate()に記述
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Dungeon01));
        }

        //ESCキーが押されたら
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
            //下層のBGMを鳴らす
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Dungeon03));
        }
        //全体の３分の１に到達したら
        else if (currentFloor == lastFloor / 3)
        {
            //Directional Lightを赤くする
            directionalLight.color = new Color32(150, 80, 80, 255);
            //中層のBGMを鳴らす
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Dungeon02));
        }

        //シーンのロード処理
        if (currentFloor == lastFloor)
        {
            //Directional Lightを暗くする
            directionalLight.color = new Color32(50, 50, 50, 255);
            //ボス戦のBGMを鳴らす
            StartCoroutine(ReserveNextBGM(MusicPlayer.BgmName.Boss));

            SceneManager.LoadScene("LastFloor");

            //プレイヤーを移動させる
            player.position = new Vector3(-6.0f, 0f, 26.0f);
            //プレイヤーを回転させる
            StartCoroutine(FaceFront());
        }
        else
        {
            //シーンのリロード
            SceneManager.LoadScene("Maze");
        }
        

        //フェードイン処理を実行する
        StartCoroutine(blackout.Fadein());
    }

    //正面を向かせる
    IEnumerator FaceFront()
    {
        //向きの自動制御が発生しないように1フレーム待つ
        yield return null;
        //変更するのはlocalRotation
        avator.localRotation = Quaternion.Euler(0f, 180.0f, 0f);

        //Vカメラの向きも変更する
        transposer.m_Heading.m_Bias = 180f;
        transposer.m_FollowOffset.y = -0.5f;
        //スクリプトのフィールド値も上書き
        vCameraController.bias = 180f;
        vCameraController.fllowOffsetY = -0.5f;
    }

    //2秒後にBGMを再生する
    IEnumerator ReserveNextBGM(MusicPlayer.BgmName bgmName, float volume = 0.5f)
    {
        yield return new WaitForSeconds(2.0f);

        musicPlayer.PlayBGM(bgmName, volume);
    }


    //----------クリア条件を満たしたら実行される処理(LastFloorManager.cs動かす)----------
    public void DisplayGameClear()
    {
        //ゲームクリアのBGMを鳴らす
        musicPlayer.PlayBGM(MusicPlayer.BgmName.GameClear, 1.0f);

        //ゲームクリアのMEを鳴らす
        musicPlayer.PlayME(MusicPlayer.MeName.GameClear, 10.0f, 1.0f);

        //オブジェクトを起動する
        GameClearController.SetActive(true);
    }


    //----------ゲームオーバーorゲームクリア時に表示されるボタン----------

    //「タイトルへ」ボタンが押されたときの処理
    public void LoadTitle()
    {
        state = State.ToTitle;
        StartCoroutine(LeaveMaze());
    }

    //「ゲームを終了する」ボタンが押されたときの処理
    public void ExitGame()
    {
        state = State.Quit;
        StartCoroutine(LeaveMaze());
    }

    IEnumerator LeaveMaze()
    {
        //クリック音を鳴らす
        musicPlayer.PlaySE(MusicPlayer.SeName.Click, 1.0f);

        yield return new WaitForSeconds(1.0f);

        //タイトル画面へ戻る
        if (state == State.ToTitle)
        {
            //カメラを一切なくすと警告文が出るので、黒壁に覆われたサブカメラを起動
            subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
            subCamera.enabled = true;

            //DontDestroyオブジェクトを全削除
            dontDestroyManager.DestroyAll();

            //タイトルへ戻る
            SceneManager.LoadScene("Title");
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
