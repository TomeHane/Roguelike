using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
	//���I�u�W�F�N�g
	Animator animator;
	Rigidbody rb;

	//�ړI�n
	Vector3 destination;
	//�X�|�[���\�G���A���m�F���ĖړI�n��ݒ肷��
	SystemManager systemManager;

	//�ړ�����
	Vector3 direction;

	//�S�̂̑��x�{��
	[SerializeField]
	float speedMagnification = 1.0f;

	//�~�܂�E�����E����̑��x�ω����������邽�߂̕ϐ�
	//Animator��Parameter�ƃ����N������
	float moveSpeed;

	//"�~�܂�"�Ƃ��̑��x
	float standSpeed = 0f;
	//"����"�Ƃ��̑��x�䗦
	[SerializeField]
	float walkSpeed = 1.0f;
	//"����"�Ƃ��̑��x�䗦(���m��)
	[SerializeField]
	float runSpeed = 2.0f;

	//�ړ����x
	Vector3 velocity;

	//animator�̃p�����[�^��
	const string PARAMETER_MOVESPEED = "moveSpeed";
	const string PARAMETER_IS_OVERLOOKING = "isOverlooking";

	//�@�����t���O
	bool flagArrived = false;



	//�f�o�b�O�p�I�u�W�F�N�g
	public GameObject hogeObject;



	void Start()
	{
		//���I�u�W�F�N�g����擾
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		//�ړI�n���X�|�[���\�G���A���烉���_���ɐݒ�
		systemManager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();

		int randomNum = Random.Range(0, systemManager.spawnablePointListX.Count);
		int x = systemManager.spawnablePointListX[randomNum];
		int y = systemManager.spawnablePointListY[randomNum];

		destination = new Vector3(systemManager.squareLength * (x - systemManager.MapWidth / 2), 0, systemManager.squareLength * (y - systemManager.MapHeight / 2));



		//�f�o�b�O�p
		Instantiate(hogeObject, destination, Quaternion.identity);



	}

	// Update is called once per frame
	void Update()
	{
		//�ړI�n�ɓ������Ă��Ȃ��Ȃ�
		if (!flagArrived)
		{
			//position���w�肵�āA�ړI�n�Ɍ�����ς��鏈��
			Vector3 targetPos = destination;
			//y���W�������Ɠ����ɂ��邱�Ƃœ񎟌��ɐ�������
			targetPos.y = transform.position.y;
			transform.LookAt(targetPos);

			//�ړ����������߂�
			//�ړ�����(���) = �ړI�n - ���ݒn
			direction = destination - transform.position;
			//y�����ւ̈ړ���0�ɂ��Ă���
			direction.y = 0f;
			//�x�N�g���̒�����1�ɂȂ�悤�ɒ���
			direction.Normalize();
			//�ړ����x�����߂�
			velocity = direction * speedMagnification;

			//Animator�̃p�����[�^�l���Z�b�g����
			moveSpeed = walkSpeed;
			animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);


			//�@�ړI�n�ɓ����������ǂ����̔���
			if (Vector3.Distance(transform.position, destination) < 5.0f)
			{
				//�����t���O�𗧂Ă�
				flagArrived = true;
				//�������~�߂�
				velocity = Vector3.zero;
				moveSpeed = standSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);
				//��������n��
				animator.SetBool(PARAMETER_IS_OVERLOOKING, true);

				//5�b��ɐV���ȖړI�n��ݒ肷��R���[�`�����΂�

			}
		}
	}


    private void FixedUpdate()
    {
		//�ړ�������
		rb.velocity = velocity;
    }
}
