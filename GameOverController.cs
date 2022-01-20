using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    //�I�u�W�F�N�g���܂Ƃ߂�e�I�u�W�F�N�g
    public GameObject parent;

    //�uGameOver�v�̊e�����̔z��
    public GameObject[] chars;

    //�u�^�C�g���ցv�{�^���Ɓu�Q�[�����I������v�{�^��
    //�R���|�[�l���g�������ŃA�^�b�`����
    public Image toTitleButtonImage;
    public Image toTitleLogoImage;
    public Button toTitleButton;

    public Image exitButtonImage;
    public Image exitLogoImage;
    public Button exitButton;

    //�����A�{�^���̈ړ����x
    public float moveSpeed = 800.0f;
    //�s�������̑��x
    public float opacifySpeed = 160.0f;
    //�{�^�����\������n�߂�܂ł̕b��
    public float waitButtonSeconds = 5.0f;

    //BGM�֌W
    [SerializeField]
    MusicPlayer musicPlayer;


    void Start()
    {
        //�e�I�u�W�F�N�g��L���ɂ���
        parent.SetActive(true);

        //BGM��ύX
        musicPlayer.PlayBGM(MusicPlayer.BgmName.GameOver, 1.0f);

        //���������A�J��Ԃ�
        for (int i = 0; i < chars.Length; i++)
        {
            float spanSeconds = 0.2f;

            //�I�u�W�F�N�g�������牟���グ��R���[�`�����J�n����
            StartCoroutine(PushUpObject(chars[i], 80.0f, (i + 1) * spanSeconds));

            //������Image�R���|�[�l���g���������\��������R���[�`�����J�n����
            StartCoroutine(OpacifyImage(chars[i].GetComponent<Image>(), 255.0f, (i + 1) * spanSeconds));

        }
        
        //�{�^���\���R���[�`�����J�n����
        StartCoroutine(DisplayButton());
    }


    //�{�^����\������
    //��莞�Ԍ�ɊJ�n���������̂ŃR���[�`�����g���Ă���
    IEnumerator DisplayButton()
    {
        yield return new WaitForSeconds(waitButtonSeconds);

        //�u�^�C�g���ցv�I�u�W�F�N�g���������s�����ɂ���
        StartCoroutine(OpacifyImage(toTitleButtonImage, 255.0f, 0f, toTitleButton));
        StartCoroutine(OpacifyImage(toTitleLogoImage, 255.0f, 0f));

        //�u�Q�[�����I������v�I�u�W�F�N�g���������s�����ɂ���
        StartCoroutine(OpacifyImage(exitButtonImage, 255.0f, 0f, exitButton));
        StartCoroutine(OpacifyImage(exitLogoImage, 255.0f, 0f));

    }


    //�I�u�W�F�N�g�������牟���グ��R���[�`��
    //�������͉����グ��ΏہA�������͎����グ�鍂��(y���W)�A��O�����͑҂�����
    IEnumerator PushUpObject(GameObject obj, float targetY, float seconds)
    {
        //�w��b���҂�
        yield return new WaitForSeconds(seconds);

        //�I�u�W�F�N�g���w�肵�����W�܂Ŏ����グ��
        //���݂�x���W��y���W���擾
        float currentX = obj.transform.localPosition.x;
        float currentY = obj.transform.localPosition.y;

        //y���W����������菬����������
        while (currentY < targetY)
        {
            //y���W���v���X
            currentY += moveSpeed * Time.deltaTime;

            //y���W�����������傫���Ȃ����ꍇ�́A��������������
            obj.transform.localPosition = new Vector3(currentX, Mathf.Min(currentY, targetY), 0f);

            //Time.deltaTime���g���Ă��邽�߁Ayield return null;���s���K�v������
            //yield return null;�́A���̃t���[���̍�Ƃ𒆒f�����̃t���[������ĊJ������
            yield return null;
        }
    }

    
    //������Image���������\��������R���[�`��
    //�������͕s�����ɂ���ΏہA�������͖ڕW�s����(��)�l�A��O�����͑҂�����
    //��l������Button��s�����ɂ���Ƃ��̂ݎg��(�f�t�H���g�l�ł�null������)
    IEnumerator OpacifyImage(Image image, float targetAlpha, float seconds, Button button = null)
    {
        //�w��b���҂�
        yield return new WaitForSeconds(seconds);

        //�u���݂̃��l�v���i�[����ϐ��p�ӂ���
        float currentAlpha = 0f;

        //Image�R���|�[�l���g��RGB���l���擾���Ă���
        Color color = image.color;

        //�I�u�W�F�N�g��s�����ɂ���while��
        //���݂̃��l���ڕW�l��菬����������
        while (currentAlpha < targetAlpha)
        {
            currentAlpha += opacifySpeed * Time.deltaTime;

            //���݂̃��l���ڕW�l���傫���Ȃ����ꍇ�́A�ڕW�l��������
            //�X�N���v�g��ł�RGB���l��0�`1�Ȃ̂�255�Ŋ����Ă�����I
            image.color = new Color(color.r, color.g, color.b, Mathf.Min(currentAlpha, targetAlpha) / 255.0f);

            //Time.deltaTime���g���Ă��邽�߁Ayield return null;���s��
            yield return null;
        }

        //�s����������������������ɓ�������
        //��l������Button�R���|�[�l���g�������Ă���Ȃ�
        if (button != null)
        {
            //Button�R���|�[�l���g��L���ɂ���
            button.enabled = true;
        }

    }
}
