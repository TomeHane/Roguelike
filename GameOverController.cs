using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    //オブジェクトをまとめる親オブジェクト
    public GameObject parent;

    //「GameOver」の各文字の配列
    public GameObject[] chars;

    //「タイトルへ」ボタンと「ゲームを終了する」ボタン
    //コンポーネントをここでアタッチする
    public Image toTitleButtonImage;
    public Image toTitleLogoImage;
    public Button toTitleButton;

    public Image exitButtonImage;
    public Image exitLogoImage;
    public Button exitButton;

    //文字、ボタンの移動速度
    public float moveSpeed = 800.0f;
    //不透明化の速度
    public float opacifySpeed = 160.0f;
    //ボタンが表示され始めるまでの秒数
    public float waitButtonSeconds = 5.0f;

    //BGM関係
    [SerializeField]
    MusicPlayer musicPlayer;


    void Start()
    {
        //親オブジェクトを有効にする
        parent.SetActive(true);

        //BGMを変更
        musicPlayer.PlayBGM(MusicPlayer.BgmName.GameOver, 1.0f);

        //文字数分、繰り返す
        for (int i = 0; i < chars.Length; i++)
        {
            float spanSeconds = 0.2f;

            //オブジェクトを下から押し上げるコルーチンを開始する
            StartCoroutine(PushUpObject(chars[i], 80.0f, (i + 1) * spanSeconds));

            //透明なImageコンポーネントを少しずつ表示させるコルーチンを開始する
            StartCoroutine(OpacifyImage(chars[i].GetComponent<Image>(), 255.0f, (i + 1) * spanSeconds));

        }
        
        //ボタン表示コルーチンを開始する
        StartCoroutine(DisplayButton());
    }


    //ボタンを表示する
    //一定時間後に開始させたいのでコルーチンを使っている
    IEnumerator DisplayButton()
    {
        yield return new WaitForSeconds(waitButtonSeconds);

        //「タイトルへ」オブジェクトを少しずつ不透明にする
        StartCoroutine(OpacifyImage(toTitleButtonImage, 255.0f, 0f, toTitleButton));
        StartCoroutine(OpacifyImage(toTitleLogoImage, 255.0f, 0f));

        //「ゲームを終了する」オブジェクトを少しずつ不透明にする
        StartCoroutine(OpacifyImage(exitButtonImage, 255.0f, 0f, exitButton));
        StartCoroutine(OpacifyImage(exitLogoImage, 255.0f, 0f));

    }


    //オブジェクトを下から押し上げるコルーチン
    //第一引数は押し上げる対象、第二引数は持ち上げる高さ(y座標)、第三引数は待ち時間
    IEnumerator PushUpObject(GameObject obj, float targetY, float seconds)
    {
        //指定秒数待つ
        yield return new WaitForSeconds(seconds);

        //オブジェクトを指定した座標まで持ち上げる
        //現在のx座標とy座標を取得
        float currentX = obj.transform.localPosition.x;
        float currentY = obj.transform.localPosition.y;

        //y座標が第二引数より小さかったら
        while (currentY < targetY)
        {
            //y座標をプラス
            currentY += moveSpeed * Time.deltaTime;

            //y座標が第二引数より大きくなった場合は、第二引数を代入する
            obj.transform.localPosition = new Vector3(currentX, Mathf.Min(currentY, targetY), 0f);

            //Time.deltaTimeを使っているため、yield return null;を行う必要がある
            //yield return null;は、そのフレームの作業を中断し次のフレームから再開させる
            yield return null;
        }
    }

    
    //透明なImageを少しずつ表示させるコルーチン
    //第一引数は不透明にする対象、第二引数は目標不透明(α)値、第三引数は待ち時間
    //第四引数はButtonを不透明にするときのみ使う(デフォルト値ではnullが入る)
    IEnumerator OpacifyImage(Image image, float targetAlpha, float seconds, Button button = null)
    {
        //指定秒数待つ
        yield return new WaitForSeconds(seconds);

        //「現在のα値」を格納する変数用意する
        float currentAlpha = 0f;

        //ImageコンポーネントのRGBα値を取得しておく
        Color color = image.color;

        //オブジェクトを不透明にするwhile文
        //現在のα値が目標値より小さかったら
        while (currentAlpha < targetAlpha)
        {
            currentAlpha += opacifySpeed * Time.deltaTime;

            //現在のα値が目標値より大きくなった場合は、目標値を代入する
            //スクリプト上でのRGBα値は0～1なので255で割ってあげる！
            image.color = new Color(color.r, color.g, color.b, Mathf.Min(currentAlpha, targetAlpha) / 255.0f);

            //Time.deltaTimeを使っているため、yield return null;を行う
            yield return null;
        }

        //不透明化処理が完了した後に動く処理
        //第四引数にButtonコンポーネントが入っているなら
        if (button != null)
        {
            //Buttonコンポーネントを有効にする
            button.enabled = true;
        }

    }
}
