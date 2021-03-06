using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWizard : MonoBehaviour
{
	//他オブジェクトをアタッチ
	[SerializeField]
	GameObject potion;

	//自オブジェクトからアタッチ
	[SerializeField]
	Rigidbody rb;
	[SerializeField]
	Collider col;
	//SearchArea.csで使用
	public Animator animator;

	//他オブジェクトからタグで検索して取得
	GameObject player;
	//スポーン可能エリアを確認して目的地を設定する
	SystemManager systemManager;

	//最新のモンスターの座標
	Vector3 _latestPos;
	//目的地
	Vector3 _destination;
	//移動方向
	Vector3 _direction;
	//移動速度
	Vector3 _velocity;

	//全体の速度倍率
	[SerializeField]
	float speedMagnification = 1.0f;

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
	float runSpeed = 4.0f;

	//animatorのパラメータ名
	const string PARAMETER_MOVESPEED = "moveSpeed";
	const string PARAMETER_IS_CHASING = "isChasing";

	//攻撃レンジ(m)
	[SerializeField]
	float attackRange = 10.0f;
	//攻撃時の静止時間(秒)
	[SerializeField]
	float attackTime = 6.0f;

	//召喚酔いフラグ
	bool isSummoningSickness = false;
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
	bool _isDead = false;

	//モンスターの状態。Commonが通常時で、Chaseが追跡時を表す。
	public enum Status
	{
		Common,
		Chase
	}
	//SearchArea.csで使用
	[System.NonSerialized]
	public Status status;

	//モンスターの種類
	public enum Species
	{
		Lower,
		Boss
	}
	[SerializeField]
	Species species = Species.Lower;

	//HP
	[SerializeField]
	float maxHp = 60.0f;
	float hp;

	//アイテムドロップ率(%)
	[SerializeField]
	float dropRate = 20.0f;

	//AudioSource
	[SerializeField]
	AudioSource audioSource;
	//AudioClip
	[SerializeField]
	AudioClip se_enemyHit1;
	[SerializeField]
	AudioClip se_enemyHit2;
	[SerializeField]
	AudioClip se_wizardDead;

	//フィールドのカプセル化
	public bool IsDead { get => _isDead; set => _isDead = value; }


	void Start()
	{
		//他オブジェクトから取得
		player = GameObject.FindGameObjectWithTag("Player");

		//Boss部屋にSystemManagerはないため
		if (species != Species.Boss)
		{
			systemManager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();
		}

		//ステータスを"通常時"で初期化
		status = Status.Common;
		//HPを初期化
		hp = maxHp;

		//召喚処理を行う
		StartCoroutine(Summon());
	}

	//召喚(酔い)処理
	IEnumerator Summon()
	{
		isSummoningSickness = true;

		yield return new WaitForSeconds(6.0f);

		isSummoningSickness = false;
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

		//召喚酔い中ならreturn;
		if (isSummoningSickness)
		{
			return;
		}
		//死亡フラグがオンならreturn;
		if (_isDead)
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
				//animatorも"通常時"に変更する
				animator.SetBool(PARAMETER_IS_CHASING, false);
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
				//animatorも"追跡時"に変更する
				animator.SetBool(PARAMETER_IS_CHASING, true);
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
				_velocity = Vector3.zero;
				moveSpeed = idleSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

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
			//向きを変える処理
			ChangeRotation();

			//目的地と使用するアニメーションから、移動方向と移動速度を決める
			DecideVelocity(player.transform.position, runSpeed);

			//攻撃範囲に入った且つ、攻撃助走中ではない場合
			if (Vector3.Distance(transform.position, player.transform.position) < attackRange && !isRunUp)
			{
				//攻撃フラグを立てる
				isAttacking = true;
				
				//プレイヤーの方を向く処理
				//プレイヤーへの矢印(ベクトル)を計算
				Vector3 direction = player.transform.position - transform.position;
				//x,z軸の回転量をゼロにするために
				direction.y = 0f;
				//プレイヤーの方に向くための角度を取得
				Quaternion rot = Quaternion.LookRotation(direction);
				//向きを変える
				transform.rotation = rot;

				//動きを止める
				_velocity = Vector3.zero;
				moveSpeed = idleSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

				//攻撃アニメーションを再生する
				animator.SetTrigger("attack");

				//一定時間後に攻撃フラグを解除するコルーチンを飛ばす
				StartCoroutine(StandStillAttacking());
			}
		}
	}


	private void FixedUpdate()
	{
		//移動させる
		rb.velocity = _velocity;
	}


	//向きを変える処理
	void ChangeRotation()
	{
		//進んでいる方向をベクトルで取得
		Vector3 diff = transform.position - _latestPos;
		//x,z軸の回転量をゼロにするために
		diff.y = 0f;
		//前回のpositionを更新
		_latestPos = transform.position;

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
		_direction = destination - transform.position;
		//y方向への移動を0にしておく
		_direction.y = 0f;
		//ベクトルの長さが1になるように調節
		_direction.Normalize();

		//moveSpeedを決める
		moveSpeed = anySpeed;
		//Animatorのパラメータ値もセットする
		animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

		//移動速度を決める
		_velocity = _direction * speedMagnification * moveSpeed;
	}


	//目的地を決める関数
	//Sensor.csからも参照する
	public void DecideDestination()
	{
		//Bossエネミーの場合
		if (species == Species.Boss)
		{
			//プレイヤーの座標を目的地とし、return;する
			_destination = player.transform.position;
			return;
		}

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
		//4秒待つ
		yield return new WaitForSeconds(4.0f);

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
		if (_isDead)
		{
			return;
		}

		//PlayerWeaponに当たった場合
		if (other.gameObject.tag == "PlayerWeapon")
		{
			//召喚酔いを解除
			if (isSummoningSickness)
			{
				isSummoningSickness = false;
			}

			//速度ベクトルをゼロにして動きを止める
			_velocity = Vector3.zero;

			//当たったゲームオブジェクトの名前を取得
			string weaponName = other.gameObject.name;
			//被ダメージを格納する変数を用意
			float getDamage = 0f;

			//オブジェクト名をもとにダメージを決定する
			switch (weaponName)
			{
				case "HitPlayerSword":
					getDamage = 10.0f;

					AudioClip damageSound = se_enemyHit1;
					//20%の確率で
					if (Random.Range(0f, 100.0f) <= 20)
					{
						damageSound = se_enemyHit2;
					}
					//被ダメSEを鳴らす
					audioSource.clip = damageSound;
					audioSource.volume = 0.5f;
					audioSource.Play();

					break;
				default:
					break;
			}

			//ウィザードにダメージを与える
			hp -= getDamage;

			//HPが0以下なら
			if (hp <= 0)
			{
				animator.SetBool("isDead", true);

				//断末魔を鳴らす
				audioSource.clip = se_wizardDead;
				audioSource.volume = 1.0f;
				audioSource.Play();

				//他オブジェクトとぶつからないよう、コライダーを切っておく
				col.enabled = false;
				//落ちていかないよう、重力を切っておく
				rb.useGravity = false;
				//一定時間後にこのオブジェクトを削除する
				StartCoroutine(DestroyThisObject());

				_isDead = true;
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

	//このオブジェクトを消すコルーチン
	IEnumerator DestroyThisObject()
	{
		yield return new WaitForSeconds(4.0f);

		//N%の確率でポーションを落とす
		if (Random.Range(0f, 100.0f) <= dropRate)
		{
			Vector3 potionPos = transform.position;
			potionPos.y = 0;
			Instantiate(potion, potionPos, Quaternion.identity);
		}

		yield return new WaitForSeconds(6.0f);

		Destroy(this.gameObject);
	}

	//被ダメフラグを解除するコルーチン
	IEnumerator ReleaseGetHit()
	{
		yield return new WaitForSeconds(1.5f);
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
}
