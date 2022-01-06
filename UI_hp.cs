using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_hp : MonoBehaviour
{
    //�e�q�I�u�W�F�N�g����A�^�b�`
    [SerializeField]
    Image hpGage;
    [SerializeField]
    PlayerController player;

    void Update()
    {
        //�c��̗͂��ŕ\��
        float percentHp = player.hp / player.maxHp * 100.0f;

        //�c��̗�(%)�ɂ���ăQ�[�W�̐F��ς���
        if (percentHp <= 20.0f)
        {
            hpGage.color = new Color32(255, 80, 80, 255);//��
        }
        else if (percentHp <= 50.0f)
        {
            hpGage.color = new Color32(255, 150, 75, 255);//�I�����W
        }
        else
        {
            hpGage.color = new Color32(100, 255, 75, 255);//��
        }

        //�Q�[�W�𑝌�������
        hpGage.fillAmount = percentHp / 100;
    }
}
