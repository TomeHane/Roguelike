using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //他オブジェクト
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameObject avatar;

    //自オブジェクト
    Rigidbody rb;
    Animator animator;

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
    //Attack01フラグ
    bool flagAttack01 = false;
    //Attack02フラグ
    bool flagAttack02 = false;

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
        //----------フラグチェック開始----------------------------------------

        //Attack01フラグが立っていて且つ、現在のステートがAttack01じゃなければ
        if (flagAttack01 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack01_SwordShield"))
        {
            //Debug.Log("Attack01フラグオフ");
            //Debug.Break();

            //Attack01フラグをオフにする
            flagAttack01 = false;
        }

        //Attack02フラグのみ立っている（Attack01再生中ではない）且つ、現在のステートがAttack02じゃなければ
        if (!flagAttack01 && flagAttack02 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack02_SwordShield"))
        {
            //Debug.Log("Attack02フラグオフ");
            //Debug.Break();

            //Attack02フラグをオフにする
            flagAttack02 = false;
        }

        //----------フラグチェック終了----------------------------------------


        //----------入力受付開始----------------------------------------

        //左Shift or mouse2(中ボタン)を押したら
        if (Input.GetButtonDown("Fire3"))
        {
            //ダッシュフラグを切り替える
            flagDash = !flagDash;
        }

        //左クリック or Enterを押したら
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackAnimationFlow());
        }

        //----------入力受付終了----------------------------------------


        //----------return処理開始----------------------------------------

        //攻撃中なら
        if (flagAttack01 || flagAttack02)
        {
            //速度ベクトルをゼロにしてreturn
            movingVelocity = Vector3.zero;
            return;
        }

        //----------return処理終了----------------------------------------


        //----------進行方向にキャラクターを向かせる処理・開始----------------------------------------

        //前回からどこに進んだかをベクトルで取得
        Vector3 diff = avatar.transform.position - latestPos;
        //前回のpositionを更新
        latestPos = avatar.transform.position;

        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        //Vector3.magnitude:ベクトルの長さをfloat型で返す
        if (diff.magnitude > 0.01)
        {
            //Quaternion.LookRotation()の引数は、Quaternion
            Quaternion rot = Quaternion.LookRotation(diff);
            //現在の向きと目標の向きとで、球面線形補間
            rot = Quaternion.Slerp(avatar.transform.rotation, rot, 0.2f);
            //向きを変える
            avatar.transform.rotation = rot;

        }

        //----------進行方向にキャラクターを向かせる処理・終了----------------------------------------


        //----------移動処理（RigidBodyのvelocityに代入する速度(ベクトル)を求める処理）・開始----------------------------------------

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


        //カメラの向きを取得し、入力値を掛けて動く方向を割り出す
        //※camera.forward[right]ベクトルの長さは１
        //カメラのZ方向のベクトル(正面方向の矢印) * WS入力値(-1.0f～1.0f)
        Vector3 movingForward = cam.transform.forward * z;
        //カメラのX方向のベクトル(右方向の矢印) * AD入力値(-1.0f～1.0f)
        Vector3 movingRight = cam.transform.right * x;

        //ベクトル合成
        Vector3 movingDirection = movingForward + movingRight;
        //宙に向かって移動しないように
        movingDirection.y = 0f;

        //ベクトルの長さが1になるように調節
        movingDirection.Normalize();

        //RigidBodyのvelocityに代入する速度(ベクトル)を求める
        movingVelocity = movingDirection * speedMagnification * moveSpeed;

        //----------移動処理（RigidBodyのvelocityに代入する速度(ベクトル)を求める処理）・終了----------------------------------------

    }


    //RigidBodyの移動は、FixedUpdate()の中にコードを書く
    private void FixedUpdate()
    {
        rb.velocity = movingVelocity;
    }


    IEnumerator AttackAnimationFlow()
    {
        //Attack01フラグが立っていなければ
        //＋Attack02フラグが立っていなければ　∵Attack02中にトリガーをセットできないようにする
        if (!flagAttack01 && !flagAttack02)
        {
            //トリガーをセットして、Attack01を再生
            animator.SetTrigger("attack1");

            yield return null;

            //1フレーム後にAttack01フラグを立てる
            flagAttack01 = true;
        }
        //Attack01フラグが立っていて且つ、Attack02フラグが立っていなければ
        else if (!flagAttack02)
        {
            //トリガーをセットしておく
            animator.SetTrigger("attack2");
            //Attack02フラグを立てる
            flagAttack02 = true;
        }

        
    }


    //デバッグ用
    private void OnGUI()
    {
        string label = $"フラグAttack01:{flagAttack01}\nフラグAttack02:{flagAttack02}";
        GUI.Label(new Rect(50, 50, 200, 60), label);
    }
}
