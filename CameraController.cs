using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player��Ǐ]����悤�ɃJ�����𓮂���
public class CameraController : MonoBehaviour
{
    Transform player;

    void Start()
    {
        //player��transform���擾����
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //�J������rotation������
        transform.rotation = Quaternion.Euler(10, 0, 0);
    }

    void Update()
    {
        //�J������position������
        transform.position = new Vector3(player.position.x + 0.6f, player.position.y + 2.0f, player.position.z - 2.6f);
    }
}
