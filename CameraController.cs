using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player‚ğ’Ç]‚·‚é‚æ‚¤‚ÉƒJƒƒ‰‚ğ“®‚©‚·
public class CameraController : MonoBehaviour
{
    Transform player;

    void Start()
    {
        //player‚Ìtransform‚ğæ“¾‚·‚é
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //ƒJƒƒ‰‚Ìrotation‚ğŒˆ’è
        transform.rotation = Quaternion.Euler(10, 0, 0);
    }

    void Update()
    {
        //ƒJƒƒ‰‚Ìposition‚ğŒˆ’è
        transform.position = new Vector3(player.position.x + 0.6f, player.position.y + 2.0f, player.position.z - 2.6f);
    }
}
