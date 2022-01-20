using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCameraController : MonoBehaviour
{
    //�A�^�b�`
    [SerializeField]
    CinemachineVirtualCamera vCamera;

    //Start()�Ŏ擾
    CinemachineOrbitalTransposer transposer;

    //Bias�l�AFollowOffset�l���o��������ϐ�
    //GameController.cs�ŎQ�Ƃ���
    [System.NonSerialized]
    public float bias;
    [System.NonSerialized]
    public float fllowOffsetY;

    //��]���x
    [SerializeField]
    float rotationalSpeedX = 100.0f;
    [SerializeField]
    float rotationalSpeedY = 5.0f;

    void Start()
    {
        //CinemachineVirtualCamera�R���|�[�l���g�́ABody����
        transposer = vCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        //Bias��Follow Offset.y�̏����l���擾
        bias = transposer.m_Heading.m_Bias;
        fllowOffsetY = transposer.m_FollowOffset.y;
    }

    void Update()
    {
        Vector3 cameraRotation = Vector3.zero;
        cameraRotation.x = Input.GetAxisRaw("Horizontal");
        cameraRotation.y = Input.GetAxisRaw("Vertical");

        if (cameraRotation.magnitude >= 0.1)
        {
            //�J�����̉�]�ʂ��v�Z����
            bias += cameraRotation.x * Time.deltaTime * rotationalSpeedX;
            fllowOffsetY -= cameraRotation.y * Time.deltaTime * rotationalSpeedY;

            //FllowOffset�̒l�ɂ͐�����������
            fllowOffsetY = Mathf.Max(fllowOffsetY, -1.9f);
            fllowOffsetY = Mathf.Min(fllowOffsetY, 7.0f);

            //Bias�𑀍�
            transposer.m_Heading.m_Bias = bias;
            //Follow Offset�𑀍�
            transposer.m_FollowOffset.y = fllowOffsetY; 
        }
    }
}
