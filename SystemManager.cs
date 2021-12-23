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
    //MoveEnemy.cs�Ŏg���̂�public
    public int MapWidth = 50;
    public int MapHeight = 50;

    //int�^�̓񎟌��z��B�v�f����MapWidth��MapHeight�Ō��肷��B
    public int[,] Map;

    int wall = 9;
    int road = 0;

    //Wall�I�u�W�F�N�g���A�^�b�`
    public GameObject WallObject;
    //���I�u�W�F�N�g���A�^�b�`
    public GameObject groundObject;
    //�ǁE���̈�ӂ̒���������
    public int squareLength;


    //�����̍����̍ŏ��E�ő�l
    [SerializeField]
    int roomMinHeight = 5;
    [SerializeField]
    int roomMaxHeight = 10;

    //�����̉����̍ŏ��E�ő�l
    [SerializeField]
    int roomMinWidth = 5;
    [SerializeField]
    int roomMaxWidth = 10;

    //�������̍ŏ��E�ő�l
    [SerializeField]
    int RoomCountMin = 10;
    [SerializeField]
    int RoomCountMax = 15;


    //���̏W���_�𑝂₵�����Ȃ炱��𑝂₷
    //2�ȏゾ�Ɠ����r�؂��\������
    const int meetPointCount = 1;


    //�X�|�[���󋵂��Ǘ����邽�߂̗񋓌^
    enum SpownArea
    {
        Disabled,
        Enabled,
        Player,
        EnemySkeleton,
        Enemy_piyo
    }

    //�}�X�ڂ��ƂɃX�|�[���󋵂����Ă���
    SpownArea[,] spownAreas;

    //�X�|�[���\�G���A���o�������郊�X�g
    //�G�l�~�[���ړI�n��ݒ肷��Ƃ��ɂ��g���̂�public
    //System.NonSerialized:�C���X�y�N�^�[��ɕ\�����Ȃ�
    [System.NonSerialized]
    public List<int> spawnablePointListX = new List<int>();
    [System.NonSerialized]
    public List<int> spawnablePointListY = new List<int>();

    //�I�u�W�F�N�g�ݒu�G���A���o�������郊�X�g
    List<int> placedPointListX = new List<int>();
    List<int> placedPointListY = new List<int>();

    //�ݒu����I�u�W�F�N�g
    public GameObject playerObject;
    public GameObject enemySkeleton;


    void Start()
    {
        ResetMapData();

        CreateSpaceData();

        CreateDangeon();

        CheckSpownArea();

        PlacedObjects();
    }

    //Map�̓񎟌��z��̏�����
    //spownAreas�̓񎟌��z��̏�����
    private void ResetMapData()
    {
        Map = new int[MapHeight, MapWidth];
        spownAreas = new SpownArea[MapHeight, MapWidth];

        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                //wall��int�l
                Map[i, j] = wall;

                //�S�}�X�̃X�|�[���󋵂�"�g�p�s��"��
                spownAreas[i, j] = SpownArea.Disabled;
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
            int roomPointY = Random.Range(2, MapHeight - roomMaxHeight - 2);

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
                    //�}�b�v�T�C�Y��50 * 50�̏ꍇ�A2500�̃I�u�W�F�N�g�𐶐�����
                    //�yWall�̈�ӂ̒����zm���z�u����Ă����B
                    Instantiate(WallObject, new Vector3(squareLength * (j - MapWidth / 2), 0, squareLength * (i - MapHeight / 2)), Quaternion.identity);
                }
                //Map[i, j]��road�Ȃ�
                else
                {
                    Instantiate(groundObject, new Vector3(squareLength * (j - MapWidth / 2), 0, squareLength * (i - MapHeight / 2)), Quaternion.identity);
                }
            }
        }
    }


    //�X�|�[���\�G���A���`�F�b�N����֐�
    void CheckSpownArea()
    {
        //���͔�����road�ł���K�v�����邽�߁A�ǍۂQ�}�X�̓`�F�b�N�ΏۊO
        for(int i = 2; i < MapHeight - 2; i++)
        {
            for (int j = 2; j < MapWidth -2; j++)
            {
                //�Y���}�X��road�Ȃ�
                if (Map[i, j] == road)
                {
                    //��U�X�|�[���\�}�X�ɂ���
                    spownAreas[i, j] = SpownArea.Enabled;

                    //���X�g�擪�ɃX�|�[���\�G���A���o��������
                    spawnablePointListX.Insert(0, j);
                    spawnablePointListY.Insert(0, i);

                    //���͔������m�F����֐�
                    CheckAllSides(i, j);
                }
            }
        }
    }

    //���͔������m�F����֐�
    void CheckAllSides(int i, int j)
    {
        for (int k = -1; k <= 1; k++)
        {
            for (int l = -1; l <= 1; l++)
            {
                //���������̂����ꂩ��road����Ȃ����
                if(Map[i + k, j + l] != road)
                {
                    //�X�|�[���s�}�X�ɂ���
                    //��"�ʘH"�ł͂Ȃ�"����"�ł̂݃X�|�[��������������
                    spownAreas[i, j] = SpownArea.Disabled;

                    //���X�g�擪�̒l���폜����
                    spawnablePointListX.RemoveAt(0);
                    spawnablePointListY.RemoveAt(0);

                    return;
                }
            }
        }
    }


    //�L�����N�^�[���̃I�u�W�F�N�g�ݒu����֐�
    void PlacedObjects()
    {
        //�v���C���[�̐ݒu�ʒu�����肷��
        DecideObjectPosition(SpownArea.Player);

        //�X�P���g���G�l�~�[�̐������肷��
        int enemySkeletonNum = Random.Range(2, 5);
        for (int i = 0; i < enemySkeletonNum; i++)
        {
            //�X�P���g���G�l�~�[�̐ݒu�ʒu�����肷��
            DecideObjectPosition(SpownArea.EnemySkeleton);
        }



        //�I�u�W�F�N�g�̐ݒu
        for (int i = 0; i < placedPointListX.Count; i++)
        {
            int x = placedPointListX[i];
            int y = placedPointListY[i];

            //�v���C���[�̐ݒu
            if (spownAreas[y, x] == SpownArea.Player)
            {
                playerObject.transform.position = new Vector3(squareLength * (x - MapWidth / 2), 0, squareLength * (y - MapHeight / 2));
                playerObject.SetActive(true);
            }
            //�X�P���g���G�l�~�[�̐���
            if (spownAreas[y, x] == SpownArea.EnemySkeleton)
            {
                Instantiate(enemySkeleton, new Vector3(squareLength * (x - MapWidth / 2), 0, squareLength * (y - MapHeight / 2)), Quaternion.identity);
            }
        }
    }

    //�I�u�W�F�N�g�̐ݒu�ʒu�����߂�֐�
    void DecideObjectPosition(SpownArea spownArea)
    {
        //�I�u�W�F�N�g�̐ݒu�ʒu�������_���Ō��肷��
        int randomNum = Random.Range(0, spawnablePointListX.Count);
        int objectPointX = spawnablePointListX[randomNum];
        int objectPointY = spawnablePointListY[randomNum];

        //�I�u�W�F�N�g�̐ݒu�ʒu�̃X�|�[���󋵂��y�Ή�������́z�ɕύX
        spownAreas[objectPointY, objectPointX] = spownArea;

        //�X�|�[���󋵂��yEnabled�z�ł͂Ȃ��Ȃ����̂ŁASpawnablePointList����폜
        spawnablePointListX.RemoveAt(randomNum);
        spawnablePointListY.RemoveAt(randomNum);

        //placedPointListX[Y]�ɃI�u�W�F�N�g�ݒu�G���A���o��������
        //�����I�ɁAspawnablePointListX[Y] �� placedPointListX[Y]�ւ̈ڂ��ւ����s���Ă���
        placedPointListX.Insert(0, objectPointX);
        placedPointListY.Insert(0, objectPointY);
    }
}
