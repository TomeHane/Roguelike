using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //���I�u�W�F�N�g
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameObject avatar;

    //���I�u�W�F�N�g
    Rigidbody rb;
    Animator animator;

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
    //Attack01�t���O
    bool flagAttack01 = false;
    //Attack02�t���O
    bool flagAttack02 = false;

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
        //----------�t���O�`�F�b�N�J�n----------------------------------------

        //Attack01�t���O�������Ă��Ċ��A���݂̃X�e�[�g��Attack01����Ȃ����
        if (flagAttack01 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack01_SwordShield"))
        {
            //Debug.Log("Attack01�t���O�I�t");
            //Debug.Break();

            //Attack01�t���O���I�t�ɂ���
            flagAttack01 = false;
        }

        //Attack02�t���O�̂ݗ����Ă���iAttack01�Đ����ł͂Ȃ��j���A���݂̃X�e�[�g��Attack02����Ȃ����
        if (!flagAttack01 && flagAttack02 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack02_SwordShield"))
        {
            //Debug.Log("Attack02�t���O�I�t");
            //Debug.Break();

            //Attack02�t���O���I�t�ɂ���
            flagAttack02 = false;
        }

        //----------�t���O�`�F�b�N�I��----------------------------------------


        //----------���͎�t�J�n----------------------------------------

        //��Shift or mouse2(���{�^��)����������
        if (Input.GetButtonDown("Fire3"))
        {
            //�_�b�V���t���O��؂�ւ���
            flagDash = !flagDash;
        }

        //���N���b�N or Enter����������
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackAnimationFlow());
        }

        //----------���͎�t�I��----------------------------------------


        //----------return�����J�n----------------------------------------

        //�U�����Ȃ�
        if (flagAttack01 || flagAttack02)
        {
            //���x�x�N�g�����[���ɂ���return
            movingVelocity = Vector3.zero;
            return;
        }

        //----------return�����I��----------------------------------------


        //----------�i�s�����ɃL�����N�^�[���������鏈���E�J�n----------------------------------------

        //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
        Vector3 diff = avatar.transform.position - latestPos;
        //�O���position���X�V
        latestPos = avatar.transform.position;

        //�x�N�g���̑傫����0.01�ȏ�̎��Ɍ�����ς��鏈��������
        //Vector3.magnitude:�x�N�g���̒�����float�^�ŕԂ�
        if (diff.magnitude > 0.01)
        {
            //Quaternion.LookRotation()�̈����́AQuaternion
            Quaternion rot = Quaternion.LookRotation(diff);
            //���݂̌����ƖڕW�̌����ƂŁA���ʐ��`���
            rot = Quaternion.Slerp(avatar.transform.rotation, rot, 0.2f);
            //������ς���
            avatar.transform.rotation = rot;

        }

        //----------�i�s�����ɃL�����N�^�[���������鏈���E�I��----------------------------------------


        //----------�ړ������iRigidBody��velocity�ɑ�����鑬�x(�x�N�g��)�����߂鏈���j�E�J�n----------------------------------------

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


        //�J�����̌������擾���A���͒l���|���ē�������������o��
        //��camera.forward[right]�x�N�g���̒����͂P
        //�J������Z�����̃x�N�g��(���ʕ����̖��) * WS���͒l(-1.0f�`1.0f)
        Vector3 movingForward = cam.transform.forward * z;
        //�J������X�����̃x�N�g��(�E�����̖��) * AD���͒l(-1.0f�`1.0f)
        Vector3 movingRight = cam.transform.right * x;

        //�x�N�g������
        Vector3 movingDirection = movingForward + movingRight;
        //���Ɍ������Ĉړ����Ȃ��悤��
        movingDirection.y = 0f;

        //�x�N�g���̒�����1�ɂȂ�悤�ɒ���
        movingDirection.Normalize();

        //RigidBody��velocity�ɑ�����鑬�x(�x�N�g��)�����߂�
        movingVelocity = movingDirection * speedMagnification * moveSpeed;

        //----------�ړ������iRigidBody��velocity�ɑ�����鑬�x(�x�N�g��)�����߂鏈���j�E�I��----------------------------------------

    }


    //RigidBody�̈ړ��́AFixedUpdate()�̒��ɃR�[�h������
    private void FixedUpdate()
    {
        rb.velocity = movingVelocity;
    }


    IEnumerator AttackAnimationFlow()
    {
        //Attack01�t���O�������Ă��Ȃ����
        //�{Attack02�t���O�������Ă��Ȃ���΁@��Attack02���Ƀg���K�[���Z�b�g�ł��Ȃ��悤�ɂ���
        if (!flagAttack01 && !flagAttack02)
        {
            //�g���K�[���Z�b�g���āAAttack01���Đ�
            animator.SetTrigger("attack1");

            yield return null;

            //1�t���[�����Attack01�t���O�𗧂Ă�
            flagAttack01 = true;
        }
        //Attack01�t���O�������Ă��Ċ��AAttack02�t���O�������Ă��Ȃ����
        else if (!flagAttack02)
        {
            //�g���K�[���Z�b�g���Ă���
            animator.SetTrigger("attack2");
            //Attack02�t���O�𗧂Ă�
            flagAttack02 = true;
        }

        
    }


    //�f�o�b�O�p
    private void OnGUI()
    {
        string label = $"�t���OAttack01:{flagAttack01}\n�t���OAttack02:{flagAttack02}";
        GUI.Label(new Rect(50, 50, 200, 60), label);
    }
}
