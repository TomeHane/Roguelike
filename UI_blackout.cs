using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_blackout : MonoBehaviour
{
    //アタッチ
    [SerializeField]
    GameController gameController;
    [SerializeField]
    PlayerController PlayerController;
    //自オブジェクトのImageコンポーネント
    [SerializeField]
    Image image;

    //Update()で一度だけ処理を行うためのフラグ
    bool isCalledOnce = false;


    private void Update()
    {
        //一度だけ行う処理
        //他スクリプトの関数を使うためUpdate()に記述
        if (!isCalledOnce)
        {
            isCalledOnce = true;

            //プレイヤーの動きを止める
            PlayerController.WaitWarp();
            //画面を少しずつ暗転させる
            StartCoroutine(Fadein());
        }
    }


    //画面を少しずつ暗転させるコルーチン
    public IEnumerator Fadeout()
    {
        //「現在のα値」を格納する変数用意する
        float currentAlpha = 0f;

        //ImageコンポーネントのRGBα値を取得しておく
        Color color = image.color;

        //オブジェクトを不透明にするwhile文
        //現在のα値が255より小さかったら
        while (currentAlpha < 255.0f)
        {
            //200,0fは定数
            currentAlpha += 200.0f * Time.deltaTime;

            //現在のα値が255より大きくなった場合は、目標値を代入する
            //スクリプト上でのRGBα値は0～1なので255で割ってあげる！
            image.color = new Color(color.r, color.g, color.b, Mathf.Min(currentAlpha, 255.0f) / 255.0f);

            //Time.deltaTimeを使っているため、yield return null;を行う
            yield return null;
        }

        //Fadeout完了後に動かす
        gameController.GoToNextFloor();
    }


    //画面を少しずつ明るくするコルーチン
    public IEnumerator Fadein()
    {
        //カメラが追いつくまで少し待つ
        yield return new WaitForSeconds(2.0f);

        float currentAlpha = 255.0f;

        Color color = image.color;

        //現在のα値が0より大きかったら
        while (currentAlpha > 0f)
        {
            currentAlpha -= 200.0f * Time.deltaTime;

            //現在のα値が0より小さくなった場合は、目標値を代入する
            image.color = new Color(color.r, color.g, color.b, Mathf.Max(currentAlpha, 0f) / 255.0f);

            //Time.deltaTimeを使っているため、yield return null;を行う
            yield return null;
        }

        //Fadein完了後に動かす
        PlayerController.isWarping = false;
    }
}
