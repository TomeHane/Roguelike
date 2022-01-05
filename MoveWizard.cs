using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWizard : MonoBehaviour
{
	//���I�u�W�F�N�g����擾
	Rigidbody rb;
	Collider col;
	//SearchArea.cs�Ŏg�p
	[System.NonSerialized]
	public Animator animator;

	//���I�u�W�F�N�g����^�O�Ō������Ď擾
	GameObject player;
	//�X�|�[���\�G���A���m�F���ĖړI�n��ݒ肷��
	SystemManager systemManager;

	//�ŐV�̃����X�^�[�̍��W
	Vector3 latestPos;
	//�ړI�n
	Vector3 _destination;
	//�ړ�����
	Vector3 direction;
	//�ړ����x
	Vector3 velocity;

	//�S�̂̑��x�{��
	[SerializeField]
	float speedMagnification = 1.0f;

	//�~�܂�E�����E����̑��x�ω����������邽�߂̕ϐ�
	//Animator��Parameter�ƃ����N������
	float moveSpeed;

	//"�~�܂�"�Ƃ��̑��x
	float idleSpeed = 0f;
	//"����"�Ƃ��̑��x�䗦
	[SerializeField]
	float walkSpeed = 1.0f;
	//"����"�Ƃ��̑��x�䗦
	[SerializeField]
	float runSpeed = 4.0f;

	//animator�̃p�����[�^��
	const string PARAMETER_MOVESPEED = "moveSpeed";
	const string PARAMETER_IS_CHASING = "isChasing";

	//�U�������W(m)
	[SerializeField]
	float attackRange = 10.0f;
	//�U�����̐Î~����(�b)
	[SerializeField]
	float attackTime = 8.0f;

	//"Update()�ň�x������������"���������邽�߂̃t���O
	bool isOnceCalled = false;
	//"�ǐՊJ�n�E�I�����Ɉ�x������������"���������邽�߂̃t���O
	bool isChasing = false;
	//(�ړI�n)�����t���O
	bool isArrived = false;
	//�U�����t���O("�U����"�ɂ͍U����̍d�����Ԃ��܂܂�Ă���)
	bool isAttacking = false;
	//�U���A�j���[�V�������s���ɗ��Ă�t���O
	bool isAttackAnimation = false;
	//(�U����)�����t���O�@�悢���Ȃ�U������ɓ���Ȃ����߂̂���
	bool isRunUp = false;
	//��_���t���O
	bool isGetHit = false;
	//���S�t���O
	bool isDead = false;

	//�����X�^�[�̏�ԁBCommon���ʏ펞�ŁAChase���ǐՎ���\���B
	public enum Status
	{
		Common,
		Chase
	}

	//SearchArea.cs�Ŏg�p
	[System.NonSerialized]
	public Status status;

	//HP
	[SerializeField]
	float maxHp = 60.0f;
	float hp;

	void Start()
	{
		//���I�u�W�F�N�g����擾
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();

		//���I�u�W�F�N�g����擾
		player = GameObject.FindGameObjectWithTag("Player");
		systemManager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();

		//�X�e�[�^�X��"�ʏ펞"�ŏ�����
		status = Status.Common;
		//HP��������
		hp = maxHp;
	}

	void Update()
	{
		//Update()�ň�x����������
		if (!isOnceCalled)
		{
			//�ړI�n�����߂�
			DecideDestination();
			//�t���O�𗧂Ă�
			isOnceCalled = true;
		}

		//���S�t���O���I���Ȃ�return;
		if (isDead)
		{
			return;
		}

		//��_���t���O���I���Ȃ�return;
		if (isGetHit)
		{
			return;
		}

		//�ʏ펞�Ȃ�
		if (status == Status.Common)
		{
			//�ǐՏI������x������������
			if (isChasing)
			{
				//animator��"�ʏ펞"�ɕύX����
				animator.SetBool(PARAMETER_IS_CHASING, false);
				//�t���O���I�t�ɂ���
				isChasing = false;
			}
			CommonMove();
		}
		//�ǐՎ��Ȃ�
		if (status == Status.Chase)
		{
			//�ǐՊJ�n����x������������
			if (!isChasing)
			{
				//animator��"�ǐՎ�"�ɕύX����
				animator.SetBool(PARAMETER_IS_CHASING, true);
				//�t���O�𗧂Ă�
				isChasing = true;
			}
			ChaseMove();
		}
	}


	//�ʏ펞�i�v���C���[�������Ă��Ȃ��Ƃ��j�̏���
	void CommonMove()
	{
		//�ړI�n�ɓ������Ă��Ȃ��Ȃ�
		if (!isArrived)
		{
			//������ς��鏈��
			ChangeRotation();

			//�ړI�n�Ǝg�p����A�j���[�V��������A�ړ������ƈړ����x�����߂�
			DecideVelocity(_destination, walkSpeed);

			//�ړI�n�ɓ��������ꍇ
			if (Vector3.Distance(transform.position, _destination) < 5.0f)
			{
				//�����t���O�𗧂Ă�
				isArrived = true;
				//�������~�߂�
				velocity = Vector3.zero;
				moveSpeed = idleSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

				//��莞�Ԍ�ɐV���ȖړI�n���Ɍ������R���[�`�����΂�
				StartCoroutine(GoNextDestination());
			}
		}
	}


	//�ǐՎ��i�v���C���[���������Ƃ��j�̏���
	void ChaseMove()
	{
		//�U���t���O���I�t�i�U���͈͊O�j�Ȃ�
		if (!isAttacking)
		{
			//������ς��鏈��
			ChangeRotation();

			//�ړI�n�Ǝg�p����A�j���[�V��������A�ړ������ƈړ����x�����߂�
			DecideVelocity(player.transform.position, runSpeed);

			//�U���͈͂ɓ��������A�U���������ł͂Ȃ��ꍇ
			if (Vector3.Distance(transform.position, player.transform.position) < attackRange && !isRunUp)
			{
				//�U���t���O�𗧂Ă�
				isAttacking = true;

				//�v���C���[�̕�������
				transform.LookAt(player.transform);

				//�������~�߂�
				velocity = Vector3.zero;
				moveSpeed = idleSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

				//�U���A�j���[�V�������Đ�����
				animator.SetTrigger("attack");

				//��莞�Ԍ�ɍU���t���O����������R���[�`�����΂�
				StartCoroutine(StandStillAttacking());
			}
		}
	}


	private void FixedUpdate()
	{
		//�ړ�������
		rb.velocity = velocity;
	}


	//������ς��鏈��
	void ChangeRotation()
	{
		//�i��ł���������x�N�g���Ŏ擾
		Vector3 diff = transform.position - latestPos;
		//�O���position���X�V
		latestPos = transform.position;

		//�x�N�g���̑傫����0.01�ȏ�Ȃ�
		if (diff.magnitude > 0.01f)
		{
			//�i��ł�������Ɍ������߂̊p�x���擾
			Quaternion rot = Quaternion.LookRotation(diff);
			//���݂̊p�x�ƖڕW�p�x�ƂŁA���ʐ��`���
			rot = Quaternion.Slerp(transform.rotation, rot, 0.2f);
			//������ς���
			transform.rotation = rot;
		}
	}


	//(�ړ�������)�ړ����x�����߂鏈��
	void DecideVelocity(Vector3 destination, float anySpeed)
	{
		//�ړ����������߂�
		//�ړ�����(���) = �ړI�n - ���ݒn
		direction = destination - transform.position;
		//y�����ւ̈ړ���0�ɂ��Ă���
		direction.y = 0f;
		//�x�N�g���̒�����1�ɂȂ�悤�ɒ���
		direction.Normalize();

		//moveSpeed�����߂�
		moveSpeed = anySpeed;
		//Animator�̃p�����[�^�l���Z�b�g����
		animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);

		//�ړ����x�����߂�
		velocity = direction * speedMagnification * moveSpeed;
	}


	//�ړI�n�����߂�֐�
	//Sensor.cs������Q�Ƃ���
	public void DecideDestination()
	{
		for (int i = 0; i < 10; i++)
		{
			//�ړI�n���X�|�[���\�G���A���烉���_���ɐݒ�
			int randomNum = Random.Range(0, systemManager.spawnablePointListX.Count);
			int x = systemManager.spawnablePointListX[randomNum];
			int y = systemManager.spawnablePointListY[randomNum];

			_destination = new Vector3(systemManager.squareLength * (x - systemManager.MapWidth / 2), 0, systemManager.squareLength * (y - systemManager.MapHeight / 2));

			//���a40m�ȓ��Ȃ�A�ړI�n���m��
			if (Vector3.Distance(transform.position, _destination) < 40.0f)
			{
				break;
			}
		}
	}

	//�V���ȖړI�n�Ɍ������֐�
	IEnumerator GoNextDestination()
	{
		//4�b�҂�
		yield return new WaitForSeconds(4.0f);

		//�ړI�n���Đݒ�
		DecideDestination();
		//�����t���O��؂�i�ړ��ĊJ�j
		isArrived = false;
	}


	//�U���̃N�[���^�C��
	IEnumerator StandStillAttacking()
	{
		//�Î~����
		yield return new WaitForSeconds(attackTime);

		//�ǐՂ̂ݍs��
		isRunUp = true;
		isAttacking = false;

		yield return new WaitForSeconds(1.0f);

		//�ǐ�&�U��
		isRunUp = false;
	}

	//�R���C�_�[���g���K�[�ɐڐG�����Ƃ��ɓ���
	private void OnTriggerEnter(Collider other)
	{
		//���S�t���O���I���Ȃ�return;
		if (isDead)
		{
			return;
		}

		//PlayerWeapon�ɓ��������ꍇ
		if (other.gameObject.tag == "PlayerWeapon")
		{
			//���x�x�N�g�����[���ɂ��ē������~�߂�
			velocity = Vector3.zero;

			//���������Q�[���I�u�W�F�N�g�̖��O���擾
			string weaponName = other.gameObject.name;
			//��_���[�W���i�[����ϐ���p��
			float getDamage = 0f;

			//�I�u�W�F�N�g�������ƂɃ_���[�W�����肷��
			switch (weaponName)
			{
				case "HitPlayerSword":
					getDamage = 10.0f;
					break;
				default:
					break;
			}

			//�v���C���[�Ƀ_���[�W��^����
			hp -= getDamage;

			Debug.Log($"�E�B�U�[�h��{getDamage}�_���[�W�I");
			Debug.Log($"�E�B�U�[�h�̎c��HP�F{hp}");

			//HP��0�ȉ��Ȃ�
			if (hp <= 0)
			{
				animator.SetBool("isDead", true);

				//���I�u�W�F�N�g�ƂԂ���Ȃ��悤�A�R���C�_�[��؂��Ă���
				col.enabled = false;
				//�����Ă����Ȃ��悤�A�d�͂�؂��Ă���
				rb.useGravity = false;
				//��莞�Ԍ�ɂ��̃I�u�W�F�N�g���폜����
				StartCoroutine(DestroyThisObject());

				isDead = true;
				return;
			}

			//�U���A�j���[�V�������ł͂Ȃ����A��_���A�j���[�V�������ł͂Ȃ��ꍇ
			if (!isAttackAnimation && !isGetHit)
			{
				animator.SetTrigger("getHit");

				//��_���A�j���[�V�������Đ�����Ă���ԁA��_���t���O�𗧂Ă�
				isGetHit = true;
				StartCoroutine(ReleaseGetHit());

				//�X�e�[�^�X���u�ǐՎ��v�ɐ؂�ւ���
				status = Status.Chase;
			}
		}
	}

	//���̃I�u�W�F�N�g�������R���[�`��
	IEnumerator DestroyThisObject()
	{
		yield return new WaitForSeconds(10.0f);

		Destroy(this.gameObject);
	}

	//��_���t���O����������R���[�`��
	IEnumerator ReleaseGetHit()
	{
		yield return new WaitForSeconds(1.5f);
		isGetHit = false;
	}


	//----------�A�j���[�V�����C�x���g�œ������֐�----------------------------------------

	void StartAttackAnimation()
	{
		isAttackAnimation = true;
	}

	void EndAttackAnimation()
	{
		isAttackAnimation = false;
	}
}