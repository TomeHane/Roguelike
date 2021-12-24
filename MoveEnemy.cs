using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
	//���I�u�W�F�N�g
	Animator animator;
	Rigidbody rb;
	//�X�|�[���\�G���A���m�F���ĖړI�n��ݒ肷��
	SystemManager systemManager;

	//�ŐV�̃����X�^�[�̍��W
	Vector3 latestPos;
	//�ړI�n
	Vector3 destination;
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

	//�A�j���[�V�����N���b�v"Overlooking"�̍Đ�����
	float clipLengthOverlooking = 0f;

	//�����t���O
	bool isArrived = false;
	//"Update()�ň�x������������"���������邽�߂̃t���O
	bool isOnceCalled = false;


	void Start()
	{
		//���I�u�W�F�N�g����擾
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		//���I�u�W�F�N�g����擾
		systemManager = GameObject.FindGameObjectWithTag("SystemManager").GetComponent<SystemManager>();

		//Overlooking�̍Đ����Ԃ��擾����
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
		//Update()�ň�x����������
        if (!isOnceCalled)
        {
			//�ړI�n�����߂�
			DecideDestination();
			//�t���O�𗧂Ă�
			isOnceCalled = true;
		}

		//�ړI�n�ɓ������Ă��Ȃ��Ȃ�
		if (!isArrived)
		{
			//������ς��鏈��
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

			//Vector3 targetPos = destination;
			//y���W�������Ɠ����ɂ��邱�Ƃœ񎟌��ɐ�������
			//targetPos.y = transform.position.y;
			//transform.LookAt(targetPos);


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
				isArrived = true;
				//�������~�߂�
				velocity = Vector3.zero;
				moveSpeed = standSpeed;
				animator.SetFloat(PARAMETER_MOVESPEED, moveSpeed);
				//��������n��
				animator.SetBool(PARAMETER_IS_OVERLOOKING, true);

				//��莞�Ԍ�ɐV���ȖړI�n���Ɍ������R���[�`�����΂�
				StartCoroutine(GoNextDestination());
			}
		}
	}


    private void FixedUpdate()
    {
		//�ړ�������
		rb.velocity = velocity;
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

			destination = new Vector3(systemManager.squareLength * (x - systemManager.MapWidth / 2), 0, systemManager.squareLength * (y - systemManager.MapHeight / 2));

			//���a40m�ȓ��Ȃ�A�ړI�n���m��
			if (Vector3.Distance(transform.position, destination) < 40.0f)
			{
				break;
			}
		}
	}

	//�V���ȖړI�n�Ɍ������֐�
	IEnumerator GoNextDestination()
    {
		//���"Overlooking"����܂ő҂�
		yield return new WaitForSeconds(clipLengthOverlooking * 2);

		//�A�j���[�V������"Stand"�ɂ���
		animator.SetBool(PARAMETER_IS_OVERLOOKING, false);

		//Overlooking��Stand�ɑJ�ڂ��I����܂�5�b���҂�
		yield return new WaitForSeconds(5.0f);

		//�ړI�n���Đݒ�
		DecideDestination();
		//�����t���O��؂�i�ړ��ĊJ�j
		isArrived = false;
    }
}
