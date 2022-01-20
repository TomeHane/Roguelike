using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Slash_player : MonoBehaviour
{
    [SerializeField]
    VisualEffect effect;

    [SerializeField]
    MusicPlayer musicPlayer;

    void Slash()
    {
        effect.SendEvent("OnPlay");
    }

    //プレイヤーの斬撃SE1
    void PlaySlashSound_1()
    {
        musicPlayer.PlaySE(MusicPlayer.SeName.PlayerSlash1);
    }

    //プレイヤーの斬撃SE2
    void PlaySlashSound_2()
    {
        musicPlayer.PlaySE(MusicPlayer.SeName.PlayerSlash2);
    }
}
