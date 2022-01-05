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
        //�a�����J��o��
        effect.SendEvent("OnPlay");
        //���̓����蔻����A�N�e�B�u��Ԃɂ���
        hitEnemySword.SetActive(true);
    }

    //���̓����蔻����A�N�e�B�u��Ԃɂ���
    void TurnOffHitEnemySword()
    {
        hitEnemySword.SetActive(false);
    }

}
