using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFloorManager : MonoBehaviour
{
    //�{�X�𒍎�����J�����I�u�W�F�N�g
    [SerializeField]
    GameObject bossCamera;

    //�Q�[���N���A�̐���
    //�^�O�Ŏ擾
    GameController gameController;
    //�G�l�~�[���Ԃ�A�^�b�`
    [SerializeField]
    List<MoveEnemy> moveEnemies;
    [SerializeField]
    List<MoveWizard> moveWizards;
    //�c��G�l�~�[��
    int remainingEnemyCount;

    //2�b��1�xUpdate()�ŏ������s��
    float currentTime = 0f;
    float spanTime = 2.0f;

    //�Q�[���N���A������
    bool isClear = false;

    void Start()
    {
        //5�b��ɃJ�����I�u�W�F�N�g���I�t�ɂ���
        StartCoroutine(TurnOffCamera());

        //�I�u�W�F�N�g���^�O�Ŏ擾
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //�c��G�l�~�[�����v�Z
        remainingEnemyCount = moveEnemies.Count + moveWizards.Count;
    }

    IEnumerator TurnOffCamera()
    {
        yield return new WaitForSeconds(5.0f);

        bossCamera.SetActive(false);
    }


    void Update()
    {
        currentTime += Time.deltaTime;

        //2�b��1�x�������s��
        if(currentTime > spanTime)
        {
            currentTime = 0f;

            for (int i = 0; i < moveEnemies.Count; i++)
            {
                //�G�l�~�[�����񂾂�A�c��G�l�~�[�������炷
                if (moveEnemies[i].IsDead)
                {
                    remainingEnemyCount--;
                    moveEnemies.RemoveAt(i);
                }
            }

            for (int i = 0; i < moveWizards.Count; i++)
            {
                //�E�B�U�[�h�����񂾂�A�c��G�l�~�[�������炷
                if (moveWizards[i].IsDead)
                {
                    remainingEnemyCount--;
                    moveWizards.RemoveAt(i);
                }
            }

            //�c��G����0�ȉ��ɂȂ�����
            if (!isClear && remainingEnemyCount <= 0)
            {
                //5�b��ɃQ�[���N���A�I�u�W�F�N�g���N������
                StartCoroutine(TurnOnGameClear());

                //�t���O�𗧂Ă�
                isClear = true;
            }
        }
    }

    IEnumerator TurnOnGameClear()
    {
        yield return new WaitForSeconds(5.0f);

        gameController.DisplayGameClear();
    }
}
