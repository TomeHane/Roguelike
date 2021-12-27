using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SearchAreaトリガーはPlayerレイヤーにしか反応しない
public class SearchArea : MonoBehaviour
{
    //親オブジェクトのスクリプトをアタッチ
    [SerializeField]
    MoveEnemy moveEnemy;
    //追跡時間
    [SerializeField]
    float chaseTime = 10.0f;

    //プレイヤーが視界に入っているかの判定
    bool isInSight = false;


    //Playerが視界に入ったら
    private void OnTriggerEnter(Collider other)
    {
        isInSight = true;

        //モンスターのステータスが通常時の場合
        if (moveEnemy.status == MoveEnemy.Status.Common)
        {
            //モンスターのステータスを"追跡時"に変更する
            moveEnemy.status = MoveEnemy.Status.Chase;
            //一定時間後にステータスの判定を行うコルーチンを動かす
            StartCoroutine(CheckStatus());
        }
    }


    //Playerが視界から外れたら
    private void OnTriggerExit(Collider other)
    {
        isInSight = false;
    }


    //ステータスを変更するかどうかを判定する
    IEnumerator CheckStatus()
    {
        yield return new WaitForSeconds(chaseTime);

        //視界内なら
        if (isInSight)
        {
            //自身のコルーチンをもう一度飛ばす
            StartCoroutine(CheckStatus());
        }
        //視界外なら
        else
        {
            //ステータスを"通常時"に戻す
            moveEnemy.status = MoveEnemy.Status.Common;
        }
    }
}
