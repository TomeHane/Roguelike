using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpCircle : MonoBehaviour
{
    //子オブジェクトをアタッチ
    [SerializeField]
    ParticleSystem ring;
    [SerializeField]
    List<GameObject> effectObjects;

    //FindGameObjectWithTag()で取得
    PlayerController playerController;
    UI_blackout blackout;

    //プレイヤー監視フラグ
    bool isWatchingPlayer = false;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        blackout = GameObject.FindGameObjectWithTag("Blackout").GetComponent<UI_blackout>();

        //PlayerControllerのフィールドにこのオブジェクトを代入する
        playerController.warpCircle = this.gameObject;

        StartCoroutine(PauseRingEffect());
    }

    //一定時間後にリングの回転を一時停止させる
    IEnumerator PauseRingEffect()
    {
        yield return new WaitForSeconds(1.0f);

        ring.Pause();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //WarpCircleの中心へ向かうフラグを立てる
            playerController.isHeadingWarpPoint = true;

            //プレイヤー監視フラグを立てる
            isWatchingPlayer = true;

            //15秒たってもワープしない場合、ここで立てたフラグを解除する
            //∵つっかえたときの予防策
            StartCoroutine(ClearFlag());
        }
    }

    public void Update()
    {
        //ワープ中なら
        //◆isWatchingPlayerを用意している理由
        //?@常にプレイヤーを監視し続けないようにするため
        //?A"一度だけ"処理を実行するため
        if (isWatchingPlayer && playerController.isWarping)
        {
            //リングの一時停止を解除
            ring.Play();

            //非アクティブだったエフェクトを、アクティブにする
            foreach (GameObject effectOjbect in effectObjects)
            {
                effectOjbect.SetActive(true);
            }

            //画面を少しずつ暗転させる
            StartCoroutine(blackout.Fadeout());

            //プレイヤー監視フラグを解除
            isWatchingPlayer = false;
        }
    }


    //フラグ解除コルーチン(保険用)
    IEnumerator ClearFlag()
    {
        yield return new WaitForSeconds(15.0f);

        //WarpCircleの中心へ向かうフラグを解除する
        playerController.isHeadingWarpPoint = false;

        //プレイヤー監視フラグを解除する
        isWatchingPlayer = false;
    }
}
