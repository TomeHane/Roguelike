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
    //���̓����蔻��
    [SerializeField]
    GameObject hitPlayerSword;
    //�Q�[���I�[�o�[��ʂ�\������I�u�W�F�N�g
    [SerializeField]
    GameObject gameOverController;

    //���I�u�W�F�N�g
    Rigidbody rb;
    Collider col;
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

    //HP(UI_hp.cs�ŎQ�Ƃ���)
    public float maxHp = 100.0f;
    [System.NonSerialized]
    public float hp;

    //�_�b�V���t���O
    bool isDash = false;
    //Attack01�t���O
    bool isAttack01 = false;
    //Attack02�t���O
    bool isAttack02 = false;
    //���G(�C���r�W�u��)�t���O
    bool isInvincible = false;
    //��_���t���O
    bool isGetHit = false;
    //���S�t���O
    bool isDead = false;


    private void Start()
    {
        //���I�u�W�F�N�g�̃R���|�[�l���g���A�C���X�y�N�^�[��ŃA�^�b�`���邱�Ƃ͂ł��Ȃ�����
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();

        //moveSpeed��������
        moveSpeed = idleSpeed;
        animator.SetFloat("moveSpeed", moveSpeed);

        //HP��������
        hp = maxHp;

        //���̓����蔻����A�N�e�B�u��Ԃɂ��Ă���
        hitPlayerSword.SetActive(false);
    }


    //"����"�̎󂯕t����Update()�ōs��
    void Update()
    {
        //----------�t���O�`�F�b�N�J�n----------------------------------------

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

        //Attack01�t���O�������Ă��Ċ��A���݂̃X�e�[�g��Attack01����Ȃ����
        if (isAttack01 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack01_SwordShield"))
        {
            //Attack01�t���O���I�t�ɂ���
            isAttack01 = false;

            //���̂Ƃ��AAttack02�t���O�������ĂȂ����
            if (!isAttack02)
            {
                //���̓����蔻����I�t�ɂ���
                hitPlayerSword.SetActive(false);
            }
        }

        //Attack02�t���O�̂ݗ����Ă���iAttack01�Đ����ł͂Ȃ��j���A���݂̃X�e�[�g��Attack02����Ȃ����
        if (!isAttack01 && isAttack02 && !animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack02_SwordShield"))
        {
            //Attack02�t���O���I�t�ɂ���
            isAttack02 = false;

            //���̓����蔻����I�t�ɂ���
            hitPlayerSword.SetActive(false);
        }

        //----------�t���O�`�F�b�N�I��----------------------------------------


        //----------���͎�t�J�n----------------------------------------

        //��Shift or mouse2(���{�^��)����������
        if (Input.GetButtonDown("Fire3"))
        {
            //�_�b�V���t���O��؂�ւ���
            isDash = !isDash;
        }

        //���N���b�N or Enter����������
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackAnimationFlow());
        }

        //----------���͎�t�I��----------------------------------------


        //----------return�����J�n----------------------------------------

        //�U�����Ȃ�
        if (isAttack01 || isAttack02)
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
        if (diff.magnitude > 0.01f)
        {
            //Quaternion.LookRotation()�̈����́AQuaternion
            Quaternion rot = Quaternion.LookRotation(diff);
            //���݂̌����ƖڕW�̌����ƂŁA���ʐ��`���
            rot = Quaternion.Slerp(avatar.transform.rotation, rot, 0.4f);
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
        else if(isDash)
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
        if (!isAttack01 && !isAttack02)
        {
            //�g���K�[���Z�b�g���āAAttack01���Đ�
            animator.SetTrigger("attack1");

            //���̓����蔻����A�N�e�B�u��Ԃɂ���
            hitPlayerSword.SetActive(true);

            yield return null;

            //1�t���[�����Attack01�t���O�𗧂Ă�
            isAttack01 = true;
        }
        //Attack01�t���O�������Ă��Ċ��AAttack02�t���O�������Ă��Ȃ����
        else if (!isAttack02)
        {
            //�g���K�[���Z�b�g���Ă���
            animator.SetTrigger("attack2");
            //Attack02�t���O�𗧂Ă�
            isAttack02 = true;
        }

        
    }


    //�R���C�_�[���g���K�[�ɐڐG�����Ƃ��ɓ���
    private void OnTriggerEnter(Collider other)
    {
        //���S�t���O���I���Ȃ�return;
        if (isDead)
        {
            return;
        }

        //���G���Ԃł͂Ȃ����AEnemyWeapon�ɓ��������ꍇ
        if (!isInvincible && other.gameObject.tag == "EnemyWeapon")
        {
            //���x�x�N�g�����[���ɂ��ē������~�߂�
            movingVelocity = Vector3.zero;

            //���������Q�[���I�u�W�F�N�g�̖��O���擾
            string weaponName = other.gameObject.name;
            //��_���[�W���i�[����ϐ���p��
            float getDamage = 0f;

            //�I�u�W�F�N�g�������ƂɃ_���[�W�����肷��
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

            //�v���C���[�Ƀ_���[�W��^����
            hp -= getDamage;

            Debug.Log($"�v���C���[��{getDamage}�_���[�W�I");
            Debug.Log($"�c��HP�F{hp}");

            //HP��0�ȉ��Ȃ�
            if (hp <= 0)
            {
                animator.SetBool("isDead", true);

                //���I�u�W�F�N�g�ƂԂ���Ȃ��悤�A�R���C�_�[��؂��Ă���
                col.enabled = false;
                //�����Ă����Ȃ��悤�A�d�͂�؂��Ă���
                rb.useGravity = false;

                isDead = true;
                return;
            }

            //��莞�Ԗ��G��Ԃɂ���
            isInvincible = true;
            StartCoroutine(ReleaseInvincible());

            //�U�����ł͂Ȃ��ꍇ
            if (!isAttack01 && !isAttack02)
            {
                animator.SetTrigger("getHit");

                //�A�j���[�V����"GetHit_SwordShield"���Đ�����Ă���ԁA��_���t���O�𗧂Ă�
                isGetHit = true;
                StartCoroutine(ReleaseGetHit());
            }
        }
    }

    //���G��Ԃ���������R���[�`��
    IEnumerator ReleaseInvincible()
    {
        yield return new WaitForSeconds(3.0f);
        isInvincible = false;
    }

    //��_���t���O����������R���[�`��
    IEnumerator ReleaseGetHit()
    {
        yield return new WaitForSeconds(0.7f);
        isGetHit = false;
    }


    //���S�A�j���[�V�����I�����ɌĂяo���֐�
    void StartGameOverController()
    {
        //�I�u�W�F�N�g���N������
        gameOverController.SetActive(true);
    }
}
