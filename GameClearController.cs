using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearController : MonoBehaviour
{
    //文字
    [SerializeField]
    List<Animator> charAnimators;
    //星エフェクト
    [SerializeField]
    GameObject starEffect;
    //エフェクトの親オブジェクト(カメラの子オブジェクト)
    [SerializeField]
    GameObject effectsParent;
    //星エフェクトを生成するスパン
    [SerializeField]
    float starSpan = 0.1f;
    float currentTime = 0f;

    //「タイトルへ」ボタンと「ゲームを終了する」ボタン
    //コンポーネントをここでアタッチする
    public Image toTitleButtonImage;
    public Image toTitleLogoImage;
    public Button toTitleButton;

    public Image exitButtonImage;
    public Image exitLogoImage;
    public Button exitButton;


    void Start()
    {
        //文字数分コルーチンを飛ばす
        for (int i = 0; i < charAnimators.Count; i++)
        {
            //文字を回転させる
            StartCoroutine(RotateChar(i * 0.5f, charAnimators[i]));
        }

        //「タイトルへ」オブジェクトを少しずつ不透明にする
        StartCoroutine(OpacifyImage(toTitleButtonImage, 200.0f, toTitleButton));
        StartCoroutine(OpacifyImage(toTitleLogoImage, 255.0f));

        //「ゲームを終了する」オブジェクトを少しずつ不透明にする
        StartCoroutine(OpacifyImage(exitButtonImage, 200.0f, exitButton));
        StartCoroutine(OpacifyImage(exitLogoImage, 255.0f));
    }


    void Update()
    {
        currentTime += Time.deltaTime;

        //一定秒数ごとに
        if (currentTime >= starSpan)
        {
            //秒数をリセット
            currentTime = 0f;

            //星エフェクトを生成する
            GameObject cloneEffect = Instantiate(starEffect, Vector3.zero, Quaternion.identity);

            //親オブジェクトを設定する
            cloneEffect.transform.parent = effectsParent.transform;

            //transformは、ローカル座標で指定する
            //座標とスケールを乱数で決める
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(-3.6f, 3.6f);
            pos.y = Random.Range(0.5f, 1.5f);
            pos.z = 4.0f;
            cloneEffect.transform.localPosition = pos;

            float randomSize = Random.Range(0.5f, 1.0f);
            cloneEffect.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

            //回転をリセットする
            cloneEffect.transform.localRotation = Quaternion.identity;

            //エフェクトを有効にする
            cloneEffect.SetActive(true);
        }
    }


    //時差式で文字を回転させるコルーチン
    IEnumerator RotateChar(float sec, Animator charAnimator)
    {
        yield return new WaitForSeconds(sec);

        charAnimator.SetBool("isRotation", true);
    }


    //透明なImageを少しずつ表示させるコルーチン
    //第一引数は不透明にする対象、第二引数は目標不透明(α)値
    //第三引数はButtonを不透明にするときのみ使う(デフォルト値ではnullが入る)
    IEnumerator OpacifyImage(Image image, float targetAlpha, Button button = null)
    {
        //指定秒数待つ
        yield return new WaitForSeconds(6.0f);

        //「現在のα値」を格納する変数用意する
        float currentAlpha = 0f;

        //ImageコンポーネントのRGBα値を取得しておく
        Color color = image.color;

        //オブジェクトを不透明にするwhile文
        //現在のα値が目標値より小さかったら
        while (currentAlpha < targetAlpha)
        {
            currentAlpha += 160.0f * Time.deltaTime;

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
