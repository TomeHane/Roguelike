/*
 *  �y��{�I�ȃ}�b�v�����̗���z
 *  �@�O���b�h�ŕǃI�u�W�F�N�g�����Ƃ���E���Ȃ��Ƃ�������߂Ă���
 *  �A�ǃI�u�W�F�N�g�𐶐�
 */

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


    //���̏W���_�𑝂₵�����Ȃ炱��𑝂₷
    [SerializeField]
    const int meetPointCount = 1;

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


    //Wall�I�u�W�F�N�g��z�u���Ȃ��ӏ������肷��
    private void CreateSpaceData()
    {
        //roomCount(�����̐�)�������_���Ō��߂�
        int roomCount = Random.Range(RoomCountMin, RoomCountMax);


        //meetPointCount(���̏W���_�̐�)�Ԃ�v�f������int�z���p�ӂ���
        int[] meetPointsX = new int[meetPointCount];
        int[] meetPointsY = new int[meetPointCount];

        //meetPointCount(���̏W���_�̐�)�Ԃ�Afor������
        for (int i = 0; i < meetPointCount; i++)
        {
            //���̏W���_�̍��W�������_���Ō��߂�
            //�͈͂̓}�b�v�S�̂�16���������Ƃ��̐^�񒆂S�G���A
            meetPointsX[i] = Random.Range(MapWidth / 4, MapWidth * 3 / 4);
            meetPointsY[i] = Random.Range(MapHeight / 4, MapHeight * 3 / 4);

            //���̏W���_�͓��R"road"�}�X
            Map[meetPointsY[i], meetPointsX[i]] = road;
        }


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


            //������"road"�Əd�Ȃ��Ă��Ȃ��ꍇ
            if (isRoad == false)
            {
                //�����쐬����
                //meetPoint�͗����Ō��߂�(0�ȏ�meetPointCount����)
                CreateRoadData(roadStartPointX, roadStartPointY, meetPointsX[Random.Range(0, meetPointCount)], meetPointsY[Random.Range(0, meetPointCount)]);
            }
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
                    //�������d�Ȃ��Ă�ꍇ�A����CreateRoadData()�͓������Ȃ��Ă悢
                    //��isRoad��true�ɂ���
                    beCreateRoad = true;
                }
                else
                {
                    //�Y���}�X��"road"�ɂ���
                    //�敔���͑S��"road"�ł�����(�}�X�͑S��wall��road)
                    Map[roomPointY + i, roomPointX + j] = road;
                }
            }
        }
        return beCreateRoad;
    }


    // ���f�[�^�𐶐�
    /// <param name="roadStartPointX"></param>
    /// <param name="roadStartPointY"></param>
    /// <param name="meetPointX"></param>
    /// <param name="meetPointY"></param>
    private void CreateRoadData(int roadStartPointX, int roadStartPointY, int meetPointX, int meetPointY)
    {
        //���̎n�_���A�W���_�����E���H
        bool isRight;
        if (roadStartPointX > meetPointX)
        {
            isRight = true;
        }
        else
        {
            isRight = false;
        }

        //���̎n�_���A�W���_���������H
        bool isUnder;
        if (roadStartPointY > meetPointY)
        {
            isUnder = false;
        }
        else
        {
            isUnder = true;
        }

        //�񕪂̈�̊m���ŁA�����ǂ̕��������邩�����߂�
        //X�������瓹�����ꍇ
        if (Random.Range(0, 2) == 0)
        {
            //���̎n�_�ƏW���_��X���W���������Ȃ����
            while (roadStartPointX != meetPointX)
            {
                //"���݂�"���̎n�_��road�}�X�ɂ���
                Map[roadStartPointY, roadStartPointX] = road;

                //���̎n�_���W���_�����E�ɂ���ꍇ
                if (isRight == true)
                {
                    //����X���W��-1����
                    roadStartPointX--;
                }
                //���̎n�_���W���_�������ɂ���ꍇ
                else
                {
                    //����X���W��+1����
                    roadStartPointX++;
                }

            }

            //���̎n�_�ƏW���_��Y���W���������Ȃ����
            while (roadStartPointY != meetPointY)
            {
                //"���݂�"���̎n�_��road�}�X�ɂ���
                Map[roadStartPointY, roadStartPointX] = road;

                //���̎n�_���W���_�������ɂ���ꍇ
                if (isUnder == true)
                {
                    //����Y���W��+1����
                    roadStartPointY++;
                }
                //���̎n�_���W���_������ɂ���ꍇ
                else
                {
                    //����Y���W��-1����
                    roadStartPointY--;
                }
            }

        }
        //Y�������瓹�����ꍇ
        else
        {

            while (roadStartPointY != meetPointY)
            {

                Map[roadStartPointY, roadStartPointX] = road;
                if (isUnder == true)
                {
                    roadStartPointY++;
                }
                else
                {
                    roadStartPointY--;
                }
            }

            while (roadStartPointX != meetPointX)
            {

                Map[roadStartPointY, roadStartPointX] = road;
                if (isRight == true)
                {
                    roadStartPointX--;
                }
                else
                {
                    roadStartPointX++;
                }

            }

        }
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
