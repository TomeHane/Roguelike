using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Slash_player : MonoBehaviour
{
    public VisualEffect effect;

    void Slash()
    {
        effect.SendEvent("OnPlay");
    }
}
