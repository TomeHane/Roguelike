using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //自オブジェクト
    Rigidbody rb;
    Animator animator;

    //動く方向
    Vector3 movingDirection;

    //移動する速度
    //rigidbody.velocityに直接代入する
    Vector3 movingVelocity;

    //最新のプレイヤーの座標
    Vector3 latestPos;

    //全体の速度倍率
    [SerializeField]
    float speedMagnification = 2.0f;

    //止まる・歩く・走るの速度変化を実現するための変数
    //AnimatorのParameterとリンクさせる
    float moveSpeed;

    //"止まる"ときの速度
    float idleSpeed = 0f;
    //"歩く"ときの速度比率
    [SerializeField]
    float walkSpeed = 1.0f;
    //"走る"ときの速度比率
    [SerializeField]
    float runSpeed = 3.0f;

    //ダッシュフラグ
    bool flagDash = false;


    private void Start()
    {
        //自オブジェクトのコンポーネントを、インスペクター上でアタッチすることはできないため
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //moveSpeedを初期化
        moveSpeed = idleSpeed;
        animator.SetFloat("moveSpeed", moveSpeed);
    }


    //"入力"の受け付けはUpdate()で行う
    void Update()
    {
        //進行方向にキャラクターを向かせる
        //前回からどこに進んだかをベクトルで取得
        Vector3 diff = transform.position - latestPos;
        //前回のpositionを更新
        latestPos = transform.position;

        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        //Vector3.magnitude:ベクトルの長さをfloat型で返す
        if (diff.magnitude > 0.01)
        {
            //Quaternion.LookRotation()の引数は、Quaternion
            Quaternion rot = Quaternion.LookRotation(diff);
            //現在の向きと目標の向きとで、球面線形補間
            rot = Quaternion.Slerp(transform.rotation, rot, 0.2f);
            //向きを変える
            transform.rotation = rot;

        }


        //左Shift or mouse2(中ボタン)を押したら
        if (Input.GetButtonDown("Fire3"))
        {
            //ダッシュフラグを切り替える
            flagDash = !flagDash;
        }


        //WASDの入力値をGetAxisで取得
        float x = Input.GetAxis("MoveX");
        float z = Input.GetAxis("MoveZ");


        //WASD入力が一切ない場合
        if (x == 0f && z == 0f)
        {
            moveSpeed = idleSpeed;
        }
        //WASD入力があり、ダッシュフラグがtrueの場合
        else if(flagDash)
        {
            moveSpeed = runSpeed;
        }
        //それ以外
        else
        {
            moveSpeed = walkSpeed;
        }

        //Animator側のmoveSpeedとリンクさせる
        animator.SetFloat("moveSpeed", moveSpeed);


        //動く方向を割り出す
        movingDirection = new Vector3 (x, 0, z);
        //ベクトルの長さが1になるように調節
        movingDirection.Normalize();
        //RigidBodyのvelocityに代入する速度(ベクトル)を求める
        movingVelocity = movingDirection * speedMagnification * moveSpeed;
    }


    //RigidBodyの移動は、FixedUpdate()の中にコードを書く
    private void FixedUpdate()
    {
        rb.velocity = movingVelocity;
    }
}
