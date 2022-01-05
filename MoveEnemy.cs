using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
	//子オブジェクトをアタッチ
	[SerializeField]
	GameObject hitEnemySword;

	//自オブジェクトから取得
	Rigidbody rb;
	Collider col;
	//SearchArea.csで使用
	[System.NonSerialized]
	public Animator animator;

	//他オブジェクトからタグで検索して取得
	GameObject player;
	//スポーン可能エリアを確認して目的地を設定する
	SystemManager systemManager;

	//最新のモンスターの座標
	Vector3 latestPos;
	//目的地
	Vector3 _destination;
	//移動方向
	Vector3 direction;
	//移動速度
	Vector3 velocity;

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
	//"走る"ときの速度比率
	[SerializeField]
	float runSpeed = 4.0f;

	//animatorのパラメータ名
	const string PARAMETER_MOVESPEED = "moveSpeed";
	const string PARAMETER_IS_OVERLOOKING = "isOverlooking";

	//アニメーションクリップ"Overlooking"の再生時間
	float clipLengthOverlooking;

	//攻撃レンジ(m)
	[SerializeField]
	float attackRange = 2.0f;
	//攻撃時の静止時間(秒)
	[SerializeField]
	float attackTime = 8.0f;

	//攻撃に移るまでの最大時間(プレイヤーに向かって走り続ける最大時間)
	[SerializeField]
	float maxRunningTime = 5.0f;
	//走り続けている時間
	float currentRunningTime = 0f;

	//"Update()で一度だけ動く処理"を実現するためのフラグ
	bool isOnceCalled = false;
	//"追跡開始・終了時に一度だけ動く処理"を実現するためのフラグ
	bool isChasing = false;
	//(目的地)到着フラグ
	bool isArrived = false;
	//攻撃中フラグ("攻撃中"には攻撃後の硬直時間も含まれている)
	bool isAttacking = false;
	//攻撃アニメーション実行中に立てるフラグ
	bool isAttackAnimation = false;
	//(攻撃の)助走フラグ　∵いきなり攻撃動作に入らないためのもの
	bool isRunUp = false;
	//被ダメフラグ
	bool isGetHit = false;
	//死亡フラグ
	bool isDead = false;

	//モンスターの状態。Commonが通常時で、Chaseが追跡時を表す。
	public enum Status
    {
		Common,
		Chase
	}

	//SearchArea.csで使用
	[System.NonSerialized]
	public Status status;

	//HP
	[SerializeField]
	float maxHp = 40.0f;
	float hp;


	void Start()
	{
		//自オブジェクトから取得
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();

		//他オブジェクトから取得
		player = GameObject.FindGameObjectWithTag("Player");
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

		//ステータスを"通常時"で初期化
		status = Status.Common;
		//HPを初期化
		hp = maxHp;
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

		//通常時なら
		if (status == Status.Common)
        {
			//追跡終了時一度だけ動く処理
			if (isChasing)
			{
				//フラグをオフにする
				isChasing = false;
			}

			CommonMove();
		}
        //追跡時なら
        if (status == Status.Chase)
        {
            //追跡開始時一度だけ動く処理
            if (!isChasing)
            {
				//"Overlooking"を解除しておく
				animator.SetBool(PARAMETER_IS_OVERLOOKING, false);
				//currentRunningTimeを初期化
				currentRunningTime = 0f;
				//フラグを立てる
				isChasing = true;
			}
			
			ChaseMove();
        }
		
	}


	//通常時（プレイヤーを見つけていないとき）の処理
	void CommonMove()
	{
		//目的地に到着していないなら
		if (!isArrived)
		{
			//向きを変える処理
			ChangeRotation();

			//目的地と使用するアニメーションから、移動方向と移動速度を決める
			DecideVelocity(_destination, walkSpeed);

			//目的地に到着した場合
			if (Vector3.Distance(transform.position, _destination) < 5.0f)
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


	//追跡時（プレイヤーを見つけたとき）の処理
	void ChaseMove()
    {
        //攻撃フラグがオフ（攻撃範囲外）なら
        if (!isAttacking)
        {
			//走り続けている時間を加算
			currentRunningTime += Time.deltaTime;

			//向きを変える処理
			ChangeRotation();

			//目的地と使用するアニメーションから、移動方向と移動速度を決める
			DecideVelocity(player.transform.position, runSpeed);

			//攻撃範囲に入った又は、一定時間以上走り続けていた場合
			if (Vector3.Distance(transform.position, player.transform.position) < attackRange 
				|| currentRunningTime >= maxRunningTime)
            {
				//攻撃助走中なら処理を中断
                if (isRunUp)
                {
					return;
                }

				//攻撃フラグを立てる
				isAttacking = true;

				//プレイヤーの方を向く
				transform.LookAt(player.transform);

				//攻撃アニメーションを再生する
				animator.SetTrigger("attack");
				//動きを止める
				velocity = Vector3.zero;
				moveSpeed = standSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

				//currentRunningTimeを初期化
				currentRunningTime = 0f;

				//一定時間後に攻撃フラグを解除するコルーチンを飛ばす
				StartCoroutine(StandStillAttacking());
			}
		}
    }


	private void FixedUpdate()
    {
		//ノックバック時(致命ダメージを受けたとき)に、後ろに移動させたいので
		//rb.velocityにVector3.zeroを代入させないようreturn;する
        if (isDead)
        {
			return;
        }

		//移動させる
		rb.velocity = velocity;
    }


	//向きを変える処理
	void ChangeRotation()
    {
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
	}


	//(移動方向と)移動速度を決める処理
	void DecideVelocity(Vector3 destination, float anySpeed)
    {
		//移動方向を決める
		//移動方向(矢印) = 目的地 - 現在地
		direction = destination - transform.position;
		//y方向への移動を0にしておく
		direction.y = 0f;
		//ベクトルの長さが1になるように調節
		direction.Normalize();

		//moveSpeedを決める
		moveSpeed = anySpeed;
		//Animatorのパラメータ値もセットする
		animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

		//移動速度を決める
		velocity = direction * speedMagnification * moveSpeed;
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

			_destination = new Vector3(systemManager.squareLength * (x - systemManager.MapWidth / 2), 0, systemManager.squareLength * (y - systemManager.MapHeight / 2));

			//半径40m以内なら、目的地を確定
			if (Vector3.Distance(transform.position, _destination) < 40.0f)
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


	//攻撃のクールタイム
	IEnumerator StandStillAttacking()
	{
		//静止する
		yield return new WaitForSeconds(attackTime);

		//追跡のみ行う
		isRunUp = true;
		isAttacking = false;

		yield return new WaitForSeconds(1.0f);

		//追跡&攻撃
		isRunUp = false;
	}


	//コライダーがトリガーに接触したときに動く
	private void OnTriggerEnter(Collider other)
	{
		//死亡フラグがオンならreturn;
		if (isDead)
		{
			return;
		}

		//PlayerWeaponに当たった場合
		if (other.gameObject.tag == "PlayerWeapon")
		{
			//速度ベクトルをゼロにして動きを止める
			velocity = Vector3.zero;

			//当たったゲームオブジェクトの名前を取得
			string weaponName = other.gameObject.name;
			//被ダメージを格納する変数を用意
			float getDamage = 0f;

			//オブジェクト名をもとにダメージを決定する
			switch (weaponName)
			{
				case "HitPlayerSword":
					getDamage = 10.0f;
					break;
				default:
					break;
			}

			//プレイヤーにダメージを与える
			hp -= getDamage;

			Debug.Log($"スケルトンに{getDamage}ダメージ！");
			Debug.Log($"スケルトンの残りHP：{hp}");

			//HPが0以下なら
			if (hp <= 0)
			{
				//剣の当たり判定を切っておく
				hitEnemySword.SetActive(false);

				//プレイヤーがいる方に向く(y座標は自身に合わせる)
				Vector3 targetPos =  player.transform.position;
				targetPos.y = transform.position.y;
				transform.LookAt(targetPos);

				animator.SetTrigger("getFatalHit");
				animator.SetBool("isDead", true);

				isDead = true;
				return;
			}

			//攻撃アニメーション中ではない且つ、被ダメアニメーション中ではない場合
			if (!isAttackAnimation && !isGetHit)
			{
				animator.SetTrigger("getHit");

				//被ダメアニメーションが再生されている間、被ダメフラグを立てる
				isGetHit = true;
				StartCoroutine(ReleaseGetHit());

				//ステータスを「追跡時」に切り替える
				status = Status.Chase;
			}
		}
	}

	//被ダメフラグを解除するコルーチン
	IEnumerator ReleaseGetHit()
	{
		yield return new WaitForSeconds(0.9f);
		isGetHit = false;
	}


	//----------アニメーションイベントで動かす関数----------------------------------------
	
	void StartAttackAnimation()
    {
		isAttackAnimation = true;
    }

	void EndAttackAnimation()
    {
		isAttackAnimation = false;
    }

	//死亡アニメーション再生終了時に動かす処理
	void EndDeathAnimation()
    {
		//他オブジェクトとぶつからないようにするため
		col.enabled = false;
		//落ちていかないよう、重力を切っておく
		rb.useGravity = false;
		//一定時間後にこのオブジェクトを削除する
		StartCoroutine(DestroyThisObject());
	}

	//このオブジェクトを消すコルーチン
	IEnumerator DestroyThisObject()
	{
		yield return new WaitForSeconds(5.0f);

		Destroy(this.gameObject);
	}
}
