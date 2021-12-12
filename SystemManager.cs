using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    //�}�b�v�S�̂̑傫��
    [SerializeField]
    const int MapWidth = 50;
    [SerializeField]
    const int MapHeight = 50;

    //int�^�̓񎟌��z��B�v�f����MapWidth��MapHeight�Ō��肷��B
    public int[,] Map;

    const int wall = 9;
    const int road = 0;

    //Wall�I�u�W�F�N�g���A�^�b�`
    public GameObject WallObject;


    //�����̍����̍ŏ��E�ő�l
    [SerializeField]
    const int roomMinHeight = 5;
    [SerializeField]
    const int roomMaxHeight = 10;

    //�����̉����̍ŏ��E�ő�l
    [SerializeField]
    const int roomMinWidth = 5;
    [SerializeField]
    const int roomMaxWidth = 10;

    //�������̍ŏ��E�ő�l
    [SerializeField]
    const int RoomCountMin = 10;
    [SerializeField]
    const int RoomCountMax = 15;

    void Start()
    {
        ResetMapData();

        CreateSpaceData();

        CreateDangeon();
    }

    // Map�̓񎟌��z��̏�����
    private void ResetMapData()
    {
        Map = new int[MapHeight, MapWidth];

        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                //wall��int�l
                Map[i, j] = wall;
            }
        }
    }


    private void CreateSpaceData()
    {
        //roomCount(�����̐�)�������_���Ō��߂�
        int roomCount = Random.Range(RoomCountMin, RoomCountMax);

        //���������A�������s��
        for (int i = 0; i < roomCount; i++)
        {
            //�����̍����A�����������_���Ō��߂�
            int roomHeight = Random.Range(roomMinHeight, roomMaxHeight);
            int roomWidth = Random.Range(roomMinWidth, roomMaxWidth);

            //�����̍쐬�n�_(����)�����߂�
            //�ǂ̒[����K��2m�����悤�ɒ���
            //�}�b�v�T�C�Y��50 * 50�̏ꍇ�A[1�`50, 1�`50]�𒲐������l�ɂȂ�
            int roomPointX = Random.Range(2, MapWidth - roomMaxWidth - 2);
            int roomPointY = Random.Range(2, MapWidth - roomMaxWidth - 2);

            //���̊J�n�n�_���A�����̃O���b�h�͈̔͂��烉���_���Ɍ��߂�
            int roadStartPointX = Random.Range(roomPointX, roomPointX + roomWidth);
            int roadStartPointY = Random.Range(roomPointY, roomPointY + roomHeight);

            //����܂ł̏������ƂɁA�������쐬����
            //���ɍ���������Əd�Ȃ����ꍇ�Atrue���Ԃ��Ă���
            bool isRoad = CreateRoomData(roomHeight, roomWidth, roomPointX, roomPointY);
        }
    }


    // �����f�[�^�𐶐��B���łɕ���������ꍇ��true��Ԃ��A�������Ȃ��悤�ɂ���
    /// <param name="roomHeight">�����̍���</param>
    /// <param name="roomWidth">�����̉���</param>
    /// <param name="roomPointX">�����̎n�_(x)</param>
    /// <param name="roomPointY">�����̎n�_(y)</param>
    /// <returns></returns>
    private bool CreateRoomData(int roomHeight, int roomWidth, int roomPointX, int roomPointY)
    {
        bool beCreateRoad = false;

        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                //�Y���}�X������"road"�������ꍇ
                if (Map[roomPointY + i, roomPointX + j] == road)
                {
                    //�߂�l��true�ɂ���
                    beCreateRoad = true;
                }
                else
                {
                    //�Y���}�X��"road"�ɂ���
                    Map[roomPointY + i, roomPointX + j] = road;
                }
            }
        }
        return beCreateRoad;
    }


    // �}�b�v�f�[�^�����ƂɃ_���W�����𐶐�
    private void CreateDangeon()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                if (Map[i, j] == wall)
                {
                    //�}�b�v�T�C�Y��50 * 50�̏ꍇ�A(-25�`24, -25�`24, 0)�̍��W��Wall�I�u�W�F�N�g�𐶐����Ă���
                    //j, i��1���ω����Ă����̂ŁAWall��1m���z�u����Ă����B
                    Instantiate(WallObject, new Vector3(j - MapWidth / 2, i - MapHeight / 2, 0), Quaternion.identity);
                }
            }
        }
    }
}
