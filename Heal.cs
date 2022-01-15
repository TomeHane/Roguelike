using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    //�e�q�I�u�W�F�N�g����A�^�b�`
    [SerializeField]
    List<ParticleSystem> healParticles;

    void Start()
    {
        //4�b��Ƀ��[�v��؂�
        StartCoroutine(LoopOff());
    }

    //���[�v��؂�R���[�`��
    IEnumerator LoopOff()
    {
        yield return new WaitForSeconds(2.0f);

        foreach (ParticleSystem particle in  healParticles)
        {
            //�p�[�e�B�N����"����"���~
            particle.Stop();
        }

        yield return new WaitForSeconds(4.0f);

        //���̃I�u�W�F�N�g���폜����
        Destroy(this.gameObject);
    }
}
