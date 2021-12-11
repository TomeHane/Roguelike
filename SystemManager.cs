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

    void Start()
    {
        ResetMapData();
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

    // �}�b�v�f�[�^�����ƂɃ_���W�����𐶐�
    private void CreateDangeon()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                if (Map[i, j] == wall)
                {
                    //�}�b�v�T�C�Y��50 * 50�̏ꍇ�A(�}25, �}25, 0)�̍��W��Wall�I�u�W�F�N�g�𐶐����Ă���
                    //j, i��1���ω����Ă����̂ŁAWall��1m���z�u����Ă����B
                    Instantiate(WallObject, new Vector3(j - MapWidth / 2, i - MapHeight / 2, 0), Quaternion.identity);
                }
            }
        }
    }
}
