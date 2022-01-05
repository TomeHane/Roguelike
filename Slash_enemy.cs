using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Slash_enemy : MonoBehaviour
{
    public VisualEffect effect;
    public GameObject hitEnemySword;

    void Slash()
    {
        //斬撃を繰り出す
        effect.SendEvent("OnPlay");
        //剣の当たり判定をアクティブ状態にする
        hitEnemySword.SetActive(true);
    }

    //剣の当たり判定を非アクティブ状態にする
    void TurnOffHitEnemySword()
    {
        hitEnemySword.SetActive(false);
    }

}
