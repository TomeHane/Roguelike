using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Slash_enemy : MonoBehaviour
{
    [SerializeField]
    VisualEffect effect;
    [SerializeField]
    GameObject hitEnemySword;

    //����AudioSource����������
    [SerializeField]
    AudioSource slashSource;
    [SerializeField]
    AudioClip se_enemySlash;

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

    //����U�鉹��炷
    void PlaySlashSound()
    {
        slashSource.clip = se_enemySlash;
        slashSource.Play();
    }
}
