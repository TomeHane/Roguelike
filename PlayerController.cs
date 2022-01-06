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
    //剣の当たり判定
    [SerializeField]
    GameObject hitPlayerSword;
    //ゲームオーバー画面を表示するオブジェクト
    [SerializeField]
    GameObject gameOverController;

    //自オブジェクト
    Rigidbody rb;
    Collider col;
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

    //HP(UI_hp.csで参照する)
    public float maxHp = 100.0f;
    [System.NonSerialized]
    public float hp;

    //ダッシュフラグ
    bool isDash = false;
    //Attack01フラグ
    bool isAttack01 = false;
    //Attack02フラグ
    bool isAttack02 = false;
    //無敵(インビジブル)フラグ
    bool isInvincible = false;
    //被ダメフラグ
    bool isGetHit = false;
    //死亡フラグ
    bool isDead = false;


    private void Start()
    {
        //自オブジェクトのコンポーネントを、インスペクター上でアタッチすることはできないため
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        //moveSpeedを初期化
        moveSpeed = idleSpeed;
        animator.SetFloat("moveSpeed", moveSpeed);

        //HPを初期化
        hp = maxHp;

        //剣の当たり判定を非アクティブ状態にしておく
        hitPlayerSword.SetActive(false);
    }


    //"入力"の受け付けはUpdate()で行う
    void Update()
    {
        //----------フラグチェック開始----------------------------------------

        //死亡フラグがオンならreturn;
        if (isDead)
        {
            return;
        }

        //被ダメフラグがオンならreturn;
        if (isGetHit)
        {
            return;
        }

        //Attack01フラグが立っていて且つ、現在のステートがAttack01じゃなければ
        if (isAttack01 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack01_SwordShield"))
        {
            //Attack01フラグをオフにする
            isAttack01 = false;

            //このとき、Attack02フラグが立ってなければ
            if (!isAttack02)
            {
                //剣の当たり判定をオフにする
                hitPlayerSword.SetActive(false);
            }
        }

        //Attack02フラグのみ立っている（Attack01再生中ではない）且つ、現在のステートがAttack02じゃなければ
        if (!isAttack01 && isAttack02 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack02_SwordShield"))
        {
            //Attack02フラグをオフにする
            isAttack02 = false;

            //剣の当たり判定をオフにする
            hitPlayerSword.SetActive(false);
        }

        //----------フラグチェック終了----------------------------------------


        //----------入力受付開始----------------------------------------

        //左Shift or mouse2(中ボタン)を押したら
        if (Input.GetButtonDown("Fire3"))
        {
            //ダッシュフラグを切り替える
            isDash = !isDash;
        }

        //左クリック or Enterを押したら
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackAnimationFlow());
        }

        //----------入力受付終了----------------------------------------


        //----------return処理開始----------------------------------------

        //攻撃中なら
        if (isAttack01 || isAttack02)
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
        if (diff.magnitude > 0.01f)
        {
            //Quaternion.LookRotation()の引数は、Quaternion
            Quaternion rot = Quaternion.LookRotation(diff);
            //現在の向きと目標の向きとで、球面線形補間
            rot = Quaternion.Slerp(avatar.transform.rotation, rot, 0.4f);
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
        else if(isDash)
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
        if (!isAttack01 && !isAttack02)
        {
            //トリガーをセットして、Attack01を再生
            animator.SetTrigger("attack1");

            //剣の当たり判定をアクティブ状態にする
            hitPlayerSword.SetActive(true);

            yield return null;

            //1フレーム後にAttack01フラグを立てる
            isAttack01 = true;
        }
        //Attack01フラグが立っていて且つ、Attack02フラグが立っていなければ
        else if (!isAttack02)
        {
            //トリガーをセットしておく
            animator.SetTrigger("attack2");
            //Attack02フラグを立てる
            isAttack02 = true;
        }

        
    }


    //コライダーがトリガーに接触したときに動く
    private void OnTriggerEnter(Collider other)
    {
        //死亡フラグがオンならreturn;
        if (isDead)
        {
            return;
        }

        //無敵時間ではない且つ、EnemyWeaponに当たった場合
        if (!isInvincible && other.gameObject.tag == "EnemyWeapon")
        {
            //速度ベクトルをゼロにして動きを止める
            movingVelocity = Vector3.zero;

            //当たったゲームオブジェクトの名前を取得
            string weaponName = other.gameObject.name;
            //被ダメージを格納する変数を用意
            float getDamage = 0f;

            //オブジェクト名をもとにダメージを決定する
            switch (weaponName)
            {
                case "HitSkeletonSword":
                    getDamage = 10.0f;
                    break;
                case "Fireball(Clone)":
                    getDamage = 15.0f;
                    break;
                default:
                    break;
            }

            //プレイヤーにダメージを与える
            hp -= getDamage;

            Debug.Log($"プレイヤーに{getDamage}ダメージ！");
            Debug.Log($"残りHP：{hp}");

            //HPが0以下なら
            if (hp <= 0)
            {
                animator.SetBool("isDead", true);

                //他オブジェクトとぶつからないよう、コライダーを切っておく
                col.enabled = false;
                //落ちていかないよう、重力を切っておく
                rb.useGravity = false;

                isDead = true;
                return;
            }

            //一定時間無敵状態にする
            isInvincible = true;
            StartCoroutine(ReleaseInvincible());

            //攻撃中ではない場合
            if (!isAttack01 && !isAttack02)
            {
                animator.SetTrigger("getHit");

                //アニメーション"GetHit_SwordShield"が再生されている間、被ダメフラグを立てる
                isGetHit = true;
                StartCoroutine(ReleaseGetHit());
            }
        }
    }

    //無敵状態を解除するコルーチン
    IEnumerator ReleaseInvincible()
    {
        yield return new WaitForSeconds(3.0f);
        isInvincible = false;
    }

    //被ダメフラグを解除するコルーチン
    IEnumerator ReleaseGetHit()
    {
        yield return new WaitForSeconds(0.7f);
        isGetHit = false;
    }


    //死亡アニメーション終了時に呼び出す関数
    void StartGameOverController()
    {
        //オブジェクトを起動する
        gameOverController.SetActive(true);
    }
}
