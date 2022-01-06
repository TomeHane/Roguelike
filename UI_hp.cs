using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_hp : MonoBehaviour
{
    //親子オブジェクトからアタッチ
    [SerializeField]
    Image hpGage;
    [SerializeField]
    PlayerController player;

    void Update()
    {
        //残り体力をで表す
        float percentHp = player.hp / player.maxHp * 100.0f;

        //残り体力(%)によってゲージの色を変える
        if (percentHp <= 20.0f)
        {
            hpGage.color = new Color32(255, 80, 80, 255);//赤
        }
        else if (percentHp <= 50.0f)
        {
            hpGage.color = new Color32(255, 150, 75, 255);//オレンジ
        }
        else
        {
            hpGage.color = new Color32(100, 255, 75, 255);//緑
        }

        //ゲージを増減させる
        hpGage.fillAmount = percentHp / 100;
    }
}
