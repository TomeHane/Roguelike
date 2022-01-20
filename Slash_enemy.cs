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

    //剣にAudioSourceを持たせる
    [SerializeField]
    AudioSource slashSource;
    [SerializeField]
    AudioClip se_enemySlash;

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

    //剣を振る音を鳴らす
    void PlaySlashSound()
    {
        slashSource.clip = se_enemySlash;
        slashSource.Play();
    }
}
