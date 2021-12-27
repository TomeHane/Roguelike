using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor_wizard : MonoBehaviour
{
    //親オブジェクトからアタッチしておく
    [SerializeField]
    MoveWizard moveWizard;

    //一定秒数、目的地の再設定を不可にする
    bool isCooled = true;

    private void OnTriggerStay(Collider other)
    {
        //壁または他のモンスターにぶつかったら
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Sensor")
        {
            //クールタイムが経過していたら
            if (isCooled)
            {
                //目的地を再取得
                moveWizard.DecideDestination();
                //一定秒数、クールタイムを設ける
                StartCoroutine(CoolTime());
            }
        }
    }


    //クールタイムを設けるコルーチン
    IEnumerator CoolTime()
    {
        isCooled = false;
        yield return new WaitForSeconds(2.0f);
        isCooled = true;
    }
}
