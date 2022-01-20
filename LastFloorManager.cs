using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFloorManager : MonoBehaviour
{
    //ボスを注視するカメラオブジェクト
    [SerializeField]
    GameObject bossCamera;

    //ゲームクリアの制御
    //タグで取得
    GameController gameController;
    //エネミー数ぶんアタッチ
    [SerializeField]
    List<MoveEnemy> moveEnemies;
    [SerializeField]
    List<MoveWizard> moveWizards;
    //残りエネミー数
    int remainingEnemyCount;

    //2秒に1度Update()で処理を行う
    float currentTime = 0f;
    float spanTime = 2.0f;

    //ゲームクリアしたか
    bool isClear = false;

    void Start()
    {
        //5秒後にカメラオブジェクトをオフにする
        StartCoroutine(TurnOffCamera());

        //オブジェクトをタグで取得
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //残りエネミー数を計算
        remainingEnemyCount = moveEnemies.Count + moveWizards.Count;
    }

    IEnumerator TurnOffCamera()
    {
        yield return new WaitForSeconds(5.0f);

        bossCamera.SetActive(false);
    }


    void Update()
    {
        currentTime += Time.deltaTime;

        //2秒に1度処理を行う
        if(currentTime > spanTime)
        {
            currentTime = 0f;

            for (int i = 0; i < moveEnemies.Count; i++)
            {
                //エネミーが死んだら、残りエネミー数を減らす
                if (moveEnemies[i].IsDead)
                {
                    remainingEnemyCount--;
                    moveEnemies.RemoveAt(i);
                }
            }

            for (int i = 0; i < moveWizards.Count; i++)
            {
                //ウィザードが死んだら、残りエネミー数を減らす
                if (moveWizards[i].IsDead)
                {
                    remainingEnemyCount--;
                    moveWizards.RemoveAt(i);
                }
            }

            //残り敵数が0以下になったら
            if (!isClear && remainingEnemyCount <= 0)
            {
                //5秒後にゲームクリアオブジェクトを起動する
                StartCoroutine(TurnOnGameClear());

                //フラグを立てる
                isClear = true;
            }
        }
    }

    IEnumerator TurnOnGameClear()
    {
        yield return new WaitForSeconds(5.0f);

        gameController.DisplayGameClear();
    }
}
