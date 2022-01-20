using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    //BGMを流すためのAudioSource
    [SerializeField]
    AudioSource bgmSource;

    //BGMのオーディオクリップのリスト
    [SerializeField]
    List<AudioClip> bgmClips;

    //BGMファイル名
    //後でstringに変換する。ToLower()するので大小文字は気にしなくてOK
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

    //MEを流すためのAudioSource
    [SerializeField]
    AudioSource meSource;

    //MEのオーディオクリップのリスト
    [SerializeField]
    List<AudioClip> meClips;

    //MEファイル名
    public enum MeName
    {
        GameClear
    }

    //SEを流すためのAudioSource
    [SerializeField]
    AudioSource seSource;

    //SEのオーディオクリップのリスト
    [SerializeField]
    List<AudioClip> seClips;

    //SEファイル名
    //プレイヤー(画面の中心)から発する音も含める
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


    //BGM名を指定してBGMを流す
    public void PlayBGM(BgmName bgmName, float volume = 0.5f)
    {
        //BGMが見つかったか
        bool result = false;

        foreach (AudioClip bgmClip in bgmClips)
        {
            //クリップ名と引数のBGM名が一致していた場合
            //右辺は、enum→string→string(小文字)と変換している
            if (bgmClip.name.ToLower() == bgmName.ToString().ToLower())
            {
                //BGMを再生
                bgmSource.clip = bgmClip;
                bgmSource.volume = volume;
                bgmSource.Play();

                //フラグを立ててbreak;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("BGMが見つかりませんでした。");
        }
    }


    //ME名を指定してMEを流す(第二引数にはMEの時間を入れる)
    public void PlayME(MeName meName, float meTime, float volume = 0.5f)
    {
        //MEが見つかったか
        bool result = false;

        foreach (AudioClip meClip in meClips)
        {
            //クリップ名と引数のBGM名が一致していた場合
            if (meClip.name.ToLower() == meName.ToString().ToLower())
            {
                //BGMの音量を覚えておく
                float bgmVolume = bgmSource.volume;
                //BGMの音量を一旦ゼロにする
                bgmSource.volume = 0f;

                //MEを再生
                meSource.clip = meClip;
                meSource.volume = volume;
                meSource.Play();

                //「MEが鳴り終わった後、BGMをフェードインさせる」コルーチンを飛ばす
                StartCoroutine(FadeInBGM(meTime, bgmVolume));

                //フラグを立ててbreak;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("MEが見つかりませんでした。");
        }
    }


    //MEが鳴り終わった後、BGMをフェードインさせる
    IEnumerator FadeInBGM(float waitSec, float targetVolume = 0.5f)
    {
        //MEが再生し終わるまで待つ
        yield return new WaitForSeconds(waitSec);

        float currentTime = 0f;
        float currentVolume = 0f;

        //現在の音量が、目標の音量より低いなら
        while (currentVolume < targetVolume)
        {
            currentTime += Time.deltaTime;

            //1秒ごとに
            if (currentTime >= 1.0f)
            {
                currentTime = 0f;

                //現在の音量を0.1上げる
                currentVolume += 0.1f;
                currentVolume = Mathf.Min(currentVolume, targetVolume);
                bgmSource.volume = currentVolume;
            }

            yield return null;
        }
    }


    //SE名を指定してSEを流す(ピッチも指定可能)
    public void PlaySE(SeName seName, float volume = 0.5f, float pitch = 1.0f)
    {
        //SEが見つかったか
        bool result = false;

        foreach (AudioClip seClip in seClips)
        {
            //クリップ名と引数のSE名が一致していた場合
            //右辺は、enum→string→string(小文字)と変換している
            if (seClip.name.ToLower() == seName.ToString().ToLower())
            {
                //SEを再生
                seSource.clip = seClip;
                seSource.volume = volume;
                seSource.pitch = pitch;
                seSource.Play();

                //フラグを立ててbreak;
                result = true;
                break;
            }
        }

        if (!result)
        {
            Debug.Log("SEが見つかりませんでした。");
        }
    }
}

