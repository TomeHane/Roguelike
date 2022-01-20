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

    //�v���C���[�̎a��SE1
    void PlaySlashSound_1()
    {
        musicPlayer.PlaySE(MusicPlayer.SeName.PlayerSlash1);
    }

    //�v���C���[�̎a��SE2
    void PlaySlashSound_2()
    {
        musicPlayer.PlaySE(MusicPlayer.SeName.PlayerSlash2);
    }
}
