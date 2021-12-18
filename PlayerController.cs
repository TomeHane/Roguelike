using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //���I�u�W�F�N�g
    Rigidbody rb;
    Animator animator;

    //��������
    Vector3 movingDirection;

    //�ړ����鑬�x
    //rigidbody.velocity�ɒ��ڑ������
    Vector3 movingVelocity;

    //�ŐV�̃v���C���[�̍��W
    Vector3 latestPos;

    //�S�̂̑��x�{��
    [SerializeField]
    float speedMagnification = 2.0f;

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
    float runSpeed = 3.0f;

    //�_�b�V���t���O
    bool flagDash = false;


    private void Start()
    {
        //���I�u�W�F�N�g�̃R���|�[�l���g���A�C���X�y�N�^�[��ŃA�^�b�`���邱�Ƃ͂ł��Ȃ�����
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //moveSpeed��������
        moveSpeed = idleSpeed;
        animator.SetFloat("moveSpeed", moveSpeed);
    }


    //"����"�̎󂯕t����Update()�ōs��
    void Update()
    {
        //�i�s�����ɃL�����N�^�[����������
        //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
        Vector3 diff = transform.position - latestPos;
        //�O���position���X�V
        latestPos = transform.position;

        //�x�N�g���̑傫����0.01�ȏ�̎��Ɍ�����ς��鏈��������
        //Vector3.magnitude:�x�N�g���̒�����float�^�ŕԂ�
        if (diff.magnitude > 0.01)
        {
            //Quaternion.LookRotation()�̈����́AQuaternion
            Quaternion rot = Quaternion.LookRotation(diff);
            //���݂̌����ƖڕW�̌����ƂŁA���ʐ��`���
            rot = Quaternion.Slerp(transform.rotation, rot, 0.2f);
            //������ς���
            transform.rotation = rot;

        }


        //��Shift or mouse2(���{�^��)����������
        if (Input.GetButtonDown("Fire3"))
        {
            //�_�b�V���t���O��؂�ւ���
            flagDash = !flagDash;
        }


        //WASD�̓��͒l��GetAxis�Ŏ擾
        float x = Input.GetAxis("MoveX");
        float z = Input.GetAxis("MoveZ");


        //WASD���͂���؂Ȃ��ꍇ
        if (x == 0f && z == 0f)
        {
            moveSpeed = idleSpeed;
        }
        //WASD���͂�����A�_�b�V���t���O��true�̏ꍇ
        else if(flagDash)
        {
            moveSpeed = runSpeed;
        }
        //����ȊO
        else
        {
            moveSpeed = walkSpeed;
        }

        //Animator����moveSpeed�ƃ����N������
        animator.SetFloat("moveSpeed", moveSpeed);


        //��������������o��
        movingDirection = new Vector3 (x, 0, z);
        //�x�N�g���̒�����1�ɂȂ�悤�ɒ���
        movingDirection.Normalize();
        //RigidBody��velocity�ɑ�����鑬�x(�x�N�g��)�����߂�
        movingVelocity = movingDirection * speedMagnification * moveSpeed;
    }


    //RigidBody�̈ړ��́AFixedUpdate()�̒��ɃR�[�h������
    private void FixedUpdate()
    {
        rb.velocity = movingVelocity;
    }
}
