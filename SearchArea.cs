using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SearchArea�g���K�[��Player���C���[�ɂ����������Ȃ�
public class SearchArea : MonoBehaviour
{
    //�e�I�u�W�F�N�g�̃X�N���v�g���A�^�b�`
    [SerializeField]
    MoveEnemy moveEnemy;
    //�ǐՎ���
    [SerializeField]
    float chaseTime = 10.0f;

    //�v���C���[�����E�ɓ����Ă��邩�̔���
    bool isInSight = false;


    //Player�����E�ɓ�������
    private void OnTriggerEnter(Collider other)
    {
        isInSight = true;

        //�����X�^�[�̃X�e�[�^�X���ʏ펞�̏ꍇ
        if (moveEnemy.status == MoveEnemy.Status.Common)
        {
            //�����X�^�[�̃X�e�[�^�X��"�ǐՎ�"�ɕύX����
            moveEnemy.status = MoveEnemy.Status.Chase;
            //��莞�Ԍ�ɃX�e�[�^�X�̔�����s���R���[�`���𓮂���
            StartCoroutine(CheckStatus());
        }
    }


    //Player�����E����O�ꂽ��
    private void OnTriggerExit(Collider other)
    {
        isInSight = false;
    }


    //�X�e�[�^�X��ύX���邩�ǂ����𔻒肷��
    IEnumerator CheckStatus()
    {
        yield return new WaitForSeconds(chaseTime);

        //���E���Ȃ�
        if (isInSight)
        {
            //���g�̃R���[�`����������x��΂�
            StartCoroutine(CheckStatus());
        }
        //���E�O�Ȃ�
        else
        {
            //�X�e�[�^�X��"�ʏ펞"�ɖ߂�
            moveEnemy.status = MoveEnemy.Status.Common;
        }
    }
}
