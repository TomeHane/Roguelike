using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
	//自オブジェクト
	Animator animator;
	Rigidbody rb;
	//スポーン可能エリアを確認して目的地を設定する
	SystemManager systemManager;

	//最新のモンスターの座標
	Vector3 latestPos;
	//目的地
	Vector3 destination;
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

	//アニメーションクリップ"Overlooking"の再生時間
	float clipLengthOverlooking = 0f;

	//到着フラグ
	bool isArrived = false;
	//"Update()で一度だけ動く処理"を実現するためのフラグ
	bool isOnceCalled = false;


	void Start()
	{
		//自オブジェクトから取得
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		//他オブジェクトから取得
		systemManager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();

		//Overlookingの再生時間を取得する
		RuntimeAnimatorController rac = animator.runtimeAnimatorController;
		AnimationClip[] clips = rac.animationClips;
		foreach (AnimationClip clip in clips)
		{
			if (clip.name == "Overlooking")
			{
				clipLengthOverlooking = clip.length;
			}
		}
	}

	void Update()
	{
		//Update()で一度だけ動かす
        if (!isOnceCalled)
        {
			//目的地を決める
			DecideDestination();
			//フラグを立てる
			isOnceCalled = true;
		}

		//目的地に到着していないなら
		if (!isArrived)
		{
			//向きを変える処理
			//進んでいる方向をベクトルで取得
			Vector3 diff = transform.position - latestPos;
			//前回のpositionを更新
			latestPos = transform.position;

			//ベクトルの大きさが0.01以上なら
            if (diff.magnitude > 0.01f)
            {
				//進んでいる方向に向くための角度を取得
				Quaternion rot = Quaternion.LookRotation(diff);
				//現在の角度と目標角度とで、球面線形補間
				rot = Quaternion.Slerp(transform.rotation, rot, 0.2f);
				//向きを変える
				transform.rotation = rot;
            }

			//Vector3 targetPos = destination;
			//y座標を自分と同じにすることで二次元に制限する
			//targetPos.y = transform.position.y;
			//transform.LookAt(targetPos);


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
				isArrived = true;
				//動きを止める
				velocity = Vector3.zero;
				moveSpeed = standSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);
				//周りを見渡す
				animator.SetBool(PARAMETER_IS_OVERLOOKING, true);

				//一定時間後に新たな目的地をに向かうコルーチンを飛ばす
				StartCoroutine(GoNextDestination());
			}
		}
	}


    private void FixedUpdate()
    {
		//移動させる
		rb.velocity = velocity;
    }


	//目的地を決める関数
	//Sensor.csからも参照する
	public void DecideDestination()
    {
		for (int i = 0; i < 10; i++)
		{
			//目的地をスポーン可能エリアからランダムに設定
			int randomNum = Random.Range(0, systemManager.spawnablePointListX.Count);
			int x = systemManager.spawnablePointListX[randomNum];
			int y = systemManager.spawnablePointListY[randomNum];

			destination = new Vector3(systemManager.squareLength * (x - systemManager.MapWidth / 2), 0, systemManager.squareLength * (y - systemManager.MapHeight / 2));

			//半径40m以内なら、目的地を確定
			if (Vector3.Distance(transform.position, destination) < 40.0f)
			{
				break;
			}
		}
	}

	//新たな目的地に向かう関数
	IEnumerator GoNextDestination()
    {
		//二回"Overlooking"するまで待つ
		yield return new WaitForSeconds(clipLengthOverlooking * 2);

		//アニメーションを"Stand"にする
		animator.SetBool(PARAMETER_IS_OVERLOOKING, false);

		//Overlooking→Standに遷移し終えるまで5秒程待つ
		yield return new WaitForSeconds(5.0f);

		//目的地を再設定
		DecideDestination();
		//到着フラグを切る（移動再開）
		isArrived = false;
    }
}
