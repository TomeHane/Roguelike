using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    //アタッチ
    public GameObject fireball;
    public Transform shooter;
    public Transform wizard;

    //火の玉の飛ばし方
    public ForceMode forceMode = ForceMode.Impulse;
    //火の玉を飛ばす速さ(ベクトルの長さ)
    public float shootMagnitude = 10.0f;

    //火の玉のrb
    Rigidbody rb;
    //火の玉を飛ばす方向
    Vector3 shootDirection;

    void Shoot()
    {
        //Fireballをプレハブから生成する
        GameObject createdBall = Instantiate(fireball, shooter.position, Quaternion.identity);
        //rbを取得
        rb = createdBall.GetComponent<Rigidbody>();

        //玉の方向は、エネミーの正面方向
        shootDirection = wizard.forward;
        shootDirection.y = 0f;
        shootDirection.Normalize();

        //生成と同時にAddForceを使って玉を発射する
        rb.AddForce(shootDirection * shootMagnitude, forceMode);
    }
}
