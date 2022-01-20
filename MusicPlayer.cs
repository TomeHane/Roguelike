using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    //BGM�𗬂����߂�AudioSource
    [SerializeField]
    AudioSource bgmSource;

    //BGM�̃I�[�f�B�I�N���b�v�̃��X�g
    [SerializeField]
    List<AudioClip> bgmClips;

    //BGM�t�@�C����
    //���string�ɕϊ�����BToLower()����̂ő召�����͋C�ɂ��Ȃ���OK
    public enum BgmName
    {
        Boss,
        Dungeon01,
        Dungeon02,
        Dungeon03,
        GameClear,
        GameOver,
        Title,
    }

    //ME�𗬂����߂�AudioSource
    [SerializeField]
    AudioSource meSource;

    //ME�̃I�[�f�B�I�N���b�v�̃��X�g
    [SerializeField]
    List<AudioClip> meClips;

    //ME�t�@�C����
    public enum MeName
    {
        GameClear
    }

    //SE�𗬂����߂�AudioSource
    [SerializeField]
    AudioSource seSource;

    //SE�̃I�[�f�B�I�N���b�v�̃��X�g
    [SerializeField]
    List<AudioClip> seClips;

    //SE�t�@�C����
    //�v���C���[(��ʂ̒��S)���甭���鉹���܂߂�
    public enum SeName
    {
        Click,
        PlayerHit,
        PlayerSlash1,
        PlayerSlash2,
        Recovery,
        Warp,
        WarpArrive
    }


    //BGM�����w�肵��BGM�𗬂�
    public void PlayBGM(BgmName bgmName, float volume = 0.5f)
    {
        //BGM������������
        bool result = false;

        foreach (AudioClip bgmClip in bgmClips)
        {
            //�N���b�v���ƈ�����BGM������v���Ă����ꍇ
            //�E�ӂ́Aenum��string��string(������)�ƕϊ����Ă���
            if (bgmClip.name.ToLower() == bgmName.ToString().ToLower())
            {
                //BGM���Đ�
                bgmSource.clip = bgmClip;
                bgmSource.volume = volume;
                bgmSource.Play();

                //�t���O�𗧂Ă�break;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("BGM��������܂���ł����B");
        }
    }


    //ME�����w�肵��ME�𗬂�(�������ɂ�ME�̎��Ԃ�����)
    public void PlayME(MeName meName, float meTime, float volume = 0.5f)
    {
        //ME������������
        bool result = false;

        foreach (AudioClip meClip in meClips)
        {
            //�N���b�v���ƈ�����BGM������v���Ă����ꍇ
            if (meClip.name.ToLower() == meName.ToString().ToLower())
            {
                //BGM�̉��ʂ��o���Ă���
                float bgmVolume = bgmSource.volume;
                //BGM�̉��ʂ���U�[���ɂ���
                bgmSource.volume = 0f;

                //ME���Đ�
                meSource.clip = meClip;
                meSource.volume = volume;
                meSource.Play();

                //�uME����I�������ABGM���t�F�[�h�C��������v�R���[�`�����΂�
                StartCoroutine(FadeInBGM(meTime, bgmVolume));

                //�t���O�𗧂Ă�break;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("ME��������܂���ł����B");
        }
    }


    //ME����I�������ABGM���t�F�[�h�C��������
    IEnumerator FadeInBGM(float waitSec, float targetVolume = 0.5f)
    {
        //ME���Đ����I���܂ő҂�
        yield return new WaitForSeconds(waitSec);

        float currentTime = 0f;
        float currentVolume = 0f;

        //���݂̉��ʂ��A�ڕW�̉��ʂ��Ⴂ�Ȃ�
        while (currentVolume < targetVolume)
        {
            currentTime += Time.deltaTime;

            //1�b���Ƃ�
            if (currentTime >= 1.0f)
            {
                currentTime = 0f;

                //���݂̉��ʂ�0.1�グ��
                currentVolume += 0.1f;
                currentVolume = Mathf.Min(currentVolume, targetVolume);
                bgmSource.volume = currentVolume;
            }

            yield return null;
        }
    }


    //SE�����w�肵��SE�𗬂�(�s�b�`���w��\)
    public void PlaySE(SeName seName, float volume = 0.5f, float pitch = 1.0f)
    {
        //SE������������
        bool result = false;

        foreach (AudioClip seClip in seClips)
        {
            //�N���b�v���ƈ�����SE������v���Ă����ꍇ
            //�E�ӂ́Aenum��string��string(������)�ƕϊ����Ă���
            if (seClip.name.ToLower() == seName.ToString().ToLower())
            {
                //SE���Đ�
                seSource.clip = seClip;
                seSource.volume = volume;
                seSource.pitch = pitch;
                seSource.Play();

                //�t���O�𗧂Ă�break;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("SE��������܂���ł����B");
        }
    }
}

