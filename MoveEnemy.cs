using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
	//自オブジェクト
	Animator animator;
	Rigidbody rb;

	//目的地
	Vector3 destination;
	//スポーン可能エリアを確認して目的地を設定する
	SystemManager systemManager;

	//移動方向
	Vector3 direction;

	//全体の速度倍率
	[SerializeField]
	float speedMagnification = 1.0f;

	//止まる・歩く・走るの速度変化を実現するための変数
	//AnimatorのParameterとリンクさせる
	float moveSpeed;

	//"止まる"ときの速度
	float standSpeed = 0f;
	//"歩く"ときの速度比率
	[SerializeField]
	float walkSpeed = 1.0f;
	//"走る"ときの速度比率(未確定)
	[SerializeField]
	float runSpeed = 2.0f;

	//移動速度
	Vector3 velocity;

	//animatorのパラメータ名
	const string PARAMETER_MOVESPEED = "moveSpeed";
	const string PARAMETER_IS_OVERLOOKING = "isOverlooking";

	//　到着フラグ
	bool flagArrived = false;



	//デバッグ用オブジェクト
	public GameObject hogeObject;



	void Start()
	{
		//自オブジェクトから取得
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		//目的地をスポーン可能エリアからランダムに設定
		systemManager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();

		int randomNum = Random.Range(0, systemManager.spawnablePointListX.Count);
		int x = systemManager.spawnablePointListX[randomNum];
		int y = systemManager.spawnablePointListY[randomNum];

		destination = new Vector3(systemManager.squareLength * (x - systemManager.MapWidth / 2), 0, systemManager.squareLength * (y - systemManager.MapHeight / 2));



		//デバッグ用
		Instantiate(hogeObject, destination, Quaternion.identity);



	}

	// Update is called once per frame
	void Update()
	{
		//目的地に到着していないなら
		if (!flagArrived)
		{
			//positionを指定して、目的地に向きを変える処理
			Vector3 targetPos = destination;
			//y座標を自分と同じにすることで二次元に制限する
			targetPos.y = transform.position.y;
			transform.LookAt(targetPos);

			//移動方向を決める
			//移動方向(矢印) = 目的地 - 現在地
			direction = destination - transform.position;
			//y方向への移動を0にしておく
			direction.y = 0f;
			//ベクトルの長さが1になるように調節
			direction.Normalize();
			//移動速度を決める
			velocity = direction * speedMagnification;

			//Animatorのパラメータ値もセットする
			moveSpeed = walkSpeed;
			animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);


			//　目的地に到着したかどうかの判定
			if (Vector3.Distance(transform.position, destination) < 5.0f)
			{
				//到着フラグを立てる
				flagArrived = true;
				//動きを止める
				velocity = Vector3.zero;
				moveSpeed = standSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);
				//周りを見渡す
				animator.SetBool(PARAMETER_IS_OVERLOOKING, true);

				//5秒後に新たな目的地を設定するコルーチンを飛ばす

			}
		}
	}


    private void FixedUpdate()
    {
		//移動させる
		rb.velocity = velocity;
    }
}
