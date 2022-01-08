using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpCircle : MonoBehaviour
{
    //�q�I�u�W�F�N�g���A�^�b�`
    [SerializeField]
    ParticleSystem ring;
    [SerializeField]
    List<GameObject> effectObjects;

    //FindGameObjectWithTag()�Ŏ擾
    PlayerController playerController;
    UI_blackout blackout;

    //�v���C���[�Ď��t���O
    bool isWatchingPlayer = false;

    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        blackout = GameObject.FindGameObjectWithTag("Blackout").GetComponent<UI_blackout>();

        //PlayerController�̃t�B�[���h�ɂ��̃I�u�W�F�N�g��������
        playerController.warpCircle = this.gameObject;

        StartCoroutine(PauseRingEffect());
    }

    //��莞�Ԍ�Ƀ����O�̉�]���ꎞ��~������
    IEnumerator PauseRingEffect()
    {
        yield return new WaitForSeconds(3.0f);

        ring.Pause();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //WarpCircle�̒��S�֌������t���O�𗧂Ă�
            playerController.isHeadingWarpPoint = true;

            //�v���C���[�Ď��t���O�𗧂Ă�
            isWatchingPlayer = true;
        }
    }

    public void Update()
    {
        //���[�v���Ȃ�
        //��isWatchingPlayer��p�ӂ��Ă��闝�R
        //�@��Ƀv���C���[���Ď��������Ȃ��悤�ɂ��邽��
        //�A"��x����"���������s���邽��
        if (isWatchingPlayer && playerController.isWarping)
        {
            //�����O�̈ꎞ��~������
            ring.Play();

            //��A�N�e�B�u�������G�t�F�N�g���A�A�N�e�B�u�ɂ���
            foreach (GameObject effectOjbect in effectObjects)
            {
                effectOjbect.SetActive(true);
            }

            //��ʂ��������Ó]������
            StartCoroutine(blackout.Fadeout());

            //�v���C���[�Ď��t���O������
            isWatchingPlayer = false;
        }
    }
}
