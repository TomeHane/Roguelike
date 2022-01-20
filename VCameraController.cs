using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCameraController : MonoBehaviour
{
    //アタッチ
    [SerializeField]
    CinemachineVirtualCamera vCamera;

    //Start()で取得
    CinemachineOrbitalTransposer transposer;

    //Bias値、FollowOffset値を覚えさせる変数
    //GameController.csで参照する
    [System.NonSerialized]
    public float bias;
    [System.NonSerialized]
    public float fllowOffsetY;

    //回転速度
    [SerializeField]
    float rotationalSpeedX = 100.0f;
    [SerializeField]
    float rotationalSpeedY = 5.0f;

    void Start()
    {
        //CinemachineVirtualCameraコンポーネントの、Body部分
        transposer = vCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        //BiasとFollow Offset.yの初期値を取得
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
            //カメラの回転量を計算する
            bias += cameraRotation.x * Time.deltaTime * rotationalSpeedX;
            fllowOffsetY -= cameraRotation.y * Time.deltaTime * rotationalSpeedY;

            //FllowOffsetの値には制限をかける
            fllowOffsetY = Mathf.Max(fllowOffsetY, -1.9f);
            fllowOffsetY = Mathf.Min(fllowOffsetY, 7.0f);

            //Biasを操作
            transposer.m_Heading.m_Bias = bias;
            //Follow Offsetを操作
            transposer.m_FollowOffset.y = fllowOffsetY; 
        }
    }
}
