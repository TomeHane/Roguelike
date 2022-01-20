using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_blackout : MonoBehaviour
{
    //�A�^�b�`
    [SerializeField]
    GameController gameController;
    [SerializeField]
    PlayerController PlayerController;
    //���I�u�W�F�N�g��Image�R���|�[�l���g
    [SerializeField]
    Image image;

    //Update()�ň�x�����������s�����߂̃t���O
    bool isCalledOnce = false;


    private void Update()
    {
        //��x�����s������
        //���X�N���v�g�̊֐����g������Update()�ɋL�q
        if (!isCalledOnce)
        {
            isCalledOnce = true;

            //�v���C���[�̓������~�߂�
            PlayerController.WaitWarp();
            //��ʂ��������Ó]������
            StartCoroutine(Fadein());
        }
    }


    //��ʂ��������Ó]������R���[�`��
    public IEnumerator Fadeout()
    {
        //�u���݂̃��l�v���i�[����ϐ��p�ӂ���
        float currentAlpha = 0f;

        //Image�R���|�[�l���g��RGB���l���擾���Ă���
        Color color = image.color;

        //�I�u�W�F�N�g��s�����ɂ���while��
        //���݂̃��l��255��菬����������
        while (currentAlpha < 255.0f)
        {
            //200,0f�͒萔
            currentAlpha += 200.0f * Time.deltaTime;

            //���݂̃��l��255���傫���Ȃ����ꍇ�́A�ڕW�l��������
            //�X�N���v�g��ł�RGB���l��0�`1�Ȃ̂�255�Ŋ����Ă�����I
            image.color = new Color(color.r, color.g, color.b, Mathf.Min(currentAlpha, 255.0f) / 255.0f);

            //Time.deltaTime���g���Ă��邽�߁Ayield return null;���s��
            yield return null;
        }

        //Fadeout������ɓ�����
        gameController.GoToNextFloor();
    }


    //��ʂ����������邭����R���[�`��
    public IEnumerator Fadein()
    {
        //�J�������ǂ����܂ŏ����҂�
        yield return new WaitForSeconds(2.0f);

        float currentAlpha = 255.0f;

        Color color = image.color;

        //���݂̃��l��0���傫��������
        while (currentAlpha > 0f)
        {
            currentAlpha -= 200.0f * Time.deltaTime;

            //���݂̃��l��0��菬�����Ȃ����ꍇ�́A�ڕW�l��������
            image.color = new Color(color.r, color.g, color.b, Mathf.Max(currentAlpha, 0f) / 255.0f);

            //Time.deltaTime���g���Ă��邽�߁Ayield return null;���s��
            yield return null;
        }

        //Fadein������ɓ�����
        PlayerController.isWarping = false;
    }
}
