using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor_wizard : MonoBehaviour
{
    //�e�I�u�W�F�N�g����A�^�b�`���Ă���
    [SerializeField]
    MoveWizard moveWizard;

    //���b���A�ړI�n�̍Đݒ��s�ɂ���
    bool isCooled = true;

    private void OnTriggerStay(Collider other)
    {
        //�ǂ܂��͑��̃����X�^�[�ɂԂ�������
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Sensor")
        {
            //�N�[���^�C�����o�߂��Ă�����
            if (isCooled)
            {
                //�ړI�n���Ď擾
                moveWizard.DecideDestination();
                //���b���A�N�[���^�C����݂���
                StartCoroutine(CoolTime());
            }
        }
    }


    //�N�[���^�C����݂���R���[�`��
    IEnumerator CoolTime()
    {
        isCooled = false;
        yield return new WaitForSeconds(2.0f);
        isCooled = true;
    }
}
