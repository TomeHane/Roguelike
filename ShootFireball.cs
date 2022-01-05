using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    //�A�^�b�`
    public GameObject fireball;
    public Transform shooter;
    public Transform wizard;

    //�΂̋ʂ̔�΂���
    public ForceMode forceMode = ForceMode.Impulse;
    //�΂̋ʂ��΂�����(�x�N�g���̒���)
    public float shootMagnitude = 10.0f;

    //�΂̋ʂ�rb
    Rigidbody rb;
    //�΂̋ʂ��΂�����
    Vector3 shootDirection;

    void Shoot()
    {
        //Fireball���v���n�u���琶������
        GameObject createdBall = Instantiate(fireball, shooter.position, Quaternion.identity);
        //rb���擾
        rb = createdBall.GetComponent<Rigidbody>();

        //�ʂ̕����́A�G�l�~�[�̐��ʕ���
        shootDirection = wizard.forward;
        shootDirection.y = 0f;
        shootDirection.Normalize();

        //�����Ɠ�����AddForce���g���ċʂ𔭎˂���
        rb.AddForce(shootDirection * shootMagnitude, forceMode);
    }
}
