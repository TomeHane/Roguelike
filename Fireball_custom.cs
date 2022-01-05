using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fireball.cs�̉�����
//���̃X�N���v�g�ł̓G�t�F�N�g�̐���̂ݍs���A�΂̋ʂ�ł��o������͑��X�N���v�g�ɔC����
public class Fireball_custom : MonoBehaviour
{
    public GameObject fieryEffect;
    public GameObject smokeEffect;
    public GameObject explodeEffect;

    Rigidbody rb;
    Collider collider;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    //�����ɏՓ˂�����
    private void OnTriggerEnter(Collider other)
    {
        //�v���C���[���ǂɂԂ�������
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Wall")
        {
            //�������Z���~����
            rb.Sleep();
            //����܂œ����Ă����G�t�F�N�g������
            if (fieryEffect != null)
            {
                Destroy(fieryEffect);
            }
            if (smokeEffect != null)
            {
                Destroy(smokeEffect);
            }
            //�����G�t�F�N�g�𓮂���
            if (explodeEffect != null)
            {
                explodeEffect.SetActive(true);
            }

            //���i�q�b�g���Ȃ��悤�ɁA�R���C�_�[�R���|�[�l���g��؂��Ă���
            collider.enabled = false;

            //5�b��ɂ��̃I�u�W�F�N�g������
            StartCoroutine(DestroyThisObject());
        }
    }

    //���̃I�u�W�F�N�g�������R���[�`��
    IEnumerator DestroyThisObject()
    {
        yield return new WaitForSeconds(5.0f);

        Destroy(this.gameObject);
    }


    //�e�I�u�W�F�N�g�ł���Fireball�I�u�W�F�N�g���L���ɂȂ�Ƃ�
    //�q�I�u�W�F�N�g�̏�Ԃ�L��[����]�ɂ���
    public void OnEnable()
    {
        if (fieryEffect != null) {
            fieryEffect.SetActive(true);
        }
        if (smokeEffect != null)
        {
            smokeEffect.SetActive(true);
        }
        if (explodeEffect != null)
        {
            explodeEffect.SetActive(false);
        }
    }
}
