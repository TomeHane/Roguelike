using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearController : MonoBehaviour
{
    //����
    [SerializeField]
    List<Animator> charAnimators;
    //���G�t�F�N�g
    [SerializeField]
    GameObject starEffect;
    //�G�t�F�N�g�̐e�I�u�W�F�N�g(�J�����̎q�I�u�W�F�N�g)
    [SerializeField]
    GameObject effectsParent;
    //���G�t�F�N�g�𐶐�����X�p��
    [SerializeField]
    float starSpan = 0.1f;
    float currentTime = 0f;

    //�u�^�C�g���ցv�{�^���Ɓu�Q�[�����I������v�{�^��
    //�R���|�[�l���g�������ŃA�^�b�`����
    public Image toTitleButtonImage;
    public Image toTitleLogoImage;
    public Button toTitleButton;

    public Image exitButtonImage;
    public Image exitLogoImage;
    public Button exitButton;


    void Start()
    {
        //���������R���[�`�����΂�
        for (int i = 0; i < charAnimators.Count; i++)
        {
            //��������]������
            StartCoroutine(RotateChar(i * 0.5f, charAnimators[i]));
        }

        //�u�^�C�g���ցv�I�u�W�F�N�g���������s�����ɂ���
        StartCoroutine(OpacifyImage(toTitleButtonImage, 200.0f, toTitleButton));
        StartCoroutine(OpacifyImage(toTitleLogoImage, 255.0f));

        //�u�Q�[�����I������v�I�u�W�F�N�g���������s�����ɂ���
        StartCoroutine(OpacifyImage(exitButtonImage, 200.0f, exitButton));
        StartCoroutine(OpacifyImage(exitLogoImage, 255.0f));
    }


    void Update()
    {
        currentTime += Time.deltaTime;

        //���b�����Ƃ�
        if (currentTime >= starSpan)
        {
            //�b�������Z�b�g
            currentTime = 0f;

            //���G�t�F�N�g�𐶐�����
            GameObject cloneEffect = Instantiate(starEffect, Vector3.zero, Quaternion.identity);

            //�e�I�u�W�F�N�g��ݒ肷��
            cloneEffect.transform.parent = effectsParent.transform;

            //transform�́A���[�J�����W�Ŏw�肷��
            //���W�ƃX�P�[���𗐐��Ō��߂�
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(-3.6f, 3.6f);
            pos.y = Random.Range(0.5f, 1.5f);
            pos.z = 4.0f;
            cloneEffect.transform.localPosition = pos;

            float randomSize = Random.Range(0.5f, 1.0f);
            cloneEffect.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

            //��]�����Z�b�g����
            cloneEffect.transform.localRotation = Quaternion.identity;

            //�G�t�F�N�g��L���ɂ���
            cloneEffect.SetActive(true);
        }
    }


    //�������ŕ�������]������R���[�`��
    IEnumerator RotateChar(float sec, Animator charAnimator)
    {
        yield return new WaitForSeconds(sec);

        charAnimator.SetBool("isRotation", true);
    }


    //������Image���������\��������R���[�`��
    //�������͕s�����ɂ���ΏہA�������͖ڕW�s����(��)�l
    //��O������Button��s�����ɂ���Ƃ��̂ݎg��(�f�t�H���g�l�ł�null������)
    IEnumerator OpacifyImage(Image image, float targetAlpha, Button button = null)
    {
        //�w��b���҂�
        yield return new WaitForSeconds(6.0f);

        //�u���݂̃��l�v���i�[����ϐ��p�ӂ���
        float currentAlpha = 0f;

        //Image�R���|�[�l���g��RGB���l���擾���Ă���
        Color color = image.color;

        //�I�u�W�F�N�g��s�����ɂ���while��
        //���݂̃��l���ڕW�l��菬����������
        while (currentAlpha < targetAlpha)
        {
            currentAlpha += 160.0f * Time.deltaTime;

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
