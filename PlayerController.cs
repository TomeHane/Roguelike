using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //�q�I�u�W�F�N�g
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameObject avatar;
    //���̓����蔻��
    [SerializeField]
    GameObject hitPlayerSword;
    //�񕜃G�t�F�N�g���o���ꏊ
    [SerializeField]
    Transform effectPoint;

    //���I�u�W�F�N�g
    //�񕜃G�t�F�N�g
    [SerializeField]
    GameObject healObject;
    //�Q�[���I�[�o�[��ʂ�\������I�u�W�F�N�g
    [SerializeField]
    GameObject gameOverController;
    //SE��炷
    [SerializeField]
    MusicPlayer musicPlayer;

    //���I�u�W�F�N�g
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Collider col;
    [SerializeField]
    Animator animator;

    //�ړ�����
    Vector3 movingDirection;
    //�ړ����鑬�x
    //rigidbody.velocity�ɒ��ڑ������
    [System.NonSerialized]
    public Vector3 movingVelocity;

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

    //WarpCircle�̒��S�֌������t���O
    //WarpCircle.cs�Ŏg�p
    [System.NonSerialized]
    public bool isHeadingWarpPoint = false;
    //���[�v���t���O
    [System.NonSerialized]
    public bool isWarping = false;

    //�ŏ��̃��[�v�t���O
    bool isFirstWarp = false;

    //�ړG��
    int collideEnemyCount = 0;

    //WarpCircle�I�u�W�F�N�g
    //WarpCircle.cs�������ɒl����������
    [System.NonSerialized]
    public GameObject warpCircle = null;

    
    private void Start()
    {
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


        //----------�i�s�����ɃL�����N�^�[���������鏈���E�J�n----------------------------------------

        //�G�ɂԂ����Ă��Ȃ����́A�Î~���Ă��Ȃ��ꍇ
        if (collideEnemyCount <= 0 || moveSpeed != idleSpeed)
        {
            //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
            Vector3 diff = avatar.transform.position - latestPos;
            //x,z���̉�]�ʂ��[���ɂ��邽�߂�
            diff.y = 0f;
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
        }

        //----------�i�s�����ɃL�����N�^�[���������鏈���E�I��----------------------------------------


        //WarpCircle�̒��S�֌������t���O���I���Ȃ�
        if (isHeadingWarpPoint)
        {
            HeadingWarpPoint();

            //�����ňړ����������̂ŁA���͎�t����O��return;
            return;
        }

        //���[�v���t���O���I���Ȃ�
        if (isWarping)
        {
            //���[�v���I���܂ŐÎ~���������̂�return;
            return;
        }


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


        //�U�����Ȃ�
        if (isAttack01 || isAttack02)
        {
            //�������~�߂�return
            moveSpeed = idleSpeed;
            animator.SetFloat("moveSpeed", moveSpeed);
            movingVelocity = Vector3.zero;
            return;
        }


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
        movingDirection = movingForward + movingRight;
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

        //���[�v�n�t���O���I���Ȃ�return;
        if (isHeadingWarpPoint || isWarping)
        {
            return;
        }

        //�|�[�V�����ɓ��������ꍇ
        if (other.gameObject.tag == "Potion")
        {
            //��SE��炷
            musicPlayer.PlaySE(MusicPlayer.SeName.Recovery);

            //�񕜏���
            hp += 50.0f;
            if (hp > maxHp)
            {
                hp = maxHp;
            }

            //�|�[�V�����I�u�W�F�N�g���폜
            Destroy(other.gameObject);

            //�񕜃G�t�F�N�g�̃I�u�W�F�N�g�𐶐����AeffectPoint�̎q�I�u�W�F�N�g�ɂ���
            Instantiate(healObject, effectPoint.position, Quaternion.identity).transform.parent = effectPoint;
        }

        //���G���Ԃł͂Ȃ����AEnemyWeapon�ɓ��������ꍇ
        if (!isInvincible && other.gameObject.tag == "EnemyWeapon")
        {
            //�������~�߂�
            moveSpeed = idleSpeed;
            animator.SetFloat("moveSpeed", moveSpeed);
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
                    //��a��SE��炷
                    musicPlayer.PlaySE(MusicPlayer.SeName.PlayerHit, 1.0f);
                    break;
                case "Fireball(Clone)":
                    getDamage = 15.0f;
                    break;
                case "FireballBoss(Clone)":
                    getDamage = 20.0f;
                    break;
                default:
                    break;
            }

            //�v���C���[�Ƀ_���[�W��^����
            hp -= getDamage;

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


    //WarpCircle�̒��S�֌������֐�
    void HeadingWarpPoint()
    {
        //�ړI�n�����߂�
        Vector3 warpPoint = warpCircle.transform.position;
        //�ړ����������߂�
        //�ړ�����(���) = �ړI�n - ���ݒn
        movingDirection = warpPoint - transform.position;
        //y�����ւ̈ړ���0�ɂ��Ă���
        movingDirection.y = 0f;
        //�x�N�g���̒�����1�ɂȂ�悤�ɒ���
        movingDirection.Normalize();

        //���点��
        moveSpeed = runSpeed;
        animator.SetFloat("moveSpeed", moveSpeed);

        //�ړ����x�����߂�
        movingVelocity = movingDirection * speedMagnification * moveSpeed;

        //WarpCircle�Ƃ̋�����1.0f�����Ȃ�
        if (Vector3.Distance(transform.position, warpPoint) < 1.0f)
        {
            WaitWarp();
        }
    }

    //���[�v�������A�������~�߂�֐�
    //UI_blackout.cs�ł��g��
    public void WaitWarp()
    {
        //�t���O��"WarpCircle�̒��S�֌�����"��"���[�v��"�ɐ؂�ւ�
        isHeadingWarpPoint = false;
        isWarping = true;

        //�v���C���[��Î~������
        moveSpeed = idleSpeed;
        animator.SetFloat("moveSpeed", moveSpeed);
        movingVelocity = Vector3.zero;

        //���[�vSE��炷
        //���P�x�ځi�P�t���A�ځj�����炳�Ȃ�
        if (!isFirstWarp)
        {
            isFirstWarp = true;
        }
        else
        {
            StartCoroutine(PlayWarpSounds());
        }
    }

    //���[�vSE��炷�R���[�`��
    IEnumerator PlayWarpSounds()
    {
        //�o�����̃��[�v��
        musicPlayer.PlaySE(MusicPlayer.SeName.Warp, 1.0f, 1.5f);

        yield return new WaitForSeconds(3.0f);

        //�������̃��[�v��
        musicPlayer.PlaySE(MusicPlayer.SeName.WarpArrive, 0.8f, 1.5f);
    }


    //-----------�ȉ��A�ړG���Ɍ������ς��Ȃ��悤�ɂ��邽�߂̏���----------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collideEnemyCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collideEnemyCount--;

            //�O�̂��߂̏���
            if (collideEnemyCount < 0)
            {
                Debug.Log("�ړG�������̐��ɂȂ�܂����B�l��0�ŏ��������܂��B");
                collideEnemyCount = 0;
            }
        }
    }
}
