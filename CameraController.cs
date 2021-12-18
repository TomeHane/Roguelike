using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//playerを追従するようにカメラを動かす
public class CameraController : MonoBehaviour
{
    Transform player;

    void Start()
    {
        //playerのtransformを取得する
        player = GameObject.FindGameObjectWithTag("Player").transform;

        //カメラのrotationを決定
        transform.rotation = Quaternion.Euler(10, 0, 0);
    }

    void Update()
    {
        //カメラのpositionを決定
        transform.position = new Vector3(player.position.x + 0.6f, player.position.y + 2.0f, player.position.z - 2.6f);
    }
}
