using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    //マップ全体の大きさ
    [SerializeField]
    const int MapWidth = 50;
    [SerializeField]
    const int MapHeight = 50;

    //int型の二次元配列。要素数はMapWidthとMapHeightで決定する。
    public int[,] Map;

    const int wall = 9;
    const int road = 0;

    //Wallオブジェクトをアタッチ
    public GameObject WallObject;

    void Start()
    {
        ResetMapData();
        CreateDangeon();
    }

    // Mapの二次元配列の初期化
    private void ResetMapData()
    {
        Map = new int[MapHeight, MapWidth];

        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                //wallはint値
                Map[i, j] = wall;
            }
        }
    }

    // マップデータをもとにダンジョンを生成
    private void CreateDangeon()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                if (Map[i, j] == wall)
                {
                    //マップサイズが50 * 50の場合、(±25, ±25, 0)の座標にWallオブジェクトを生成している
                    //j, iが1ずつ変化していくので、Wallも1mずつ配置されていく。
                    Instantiate(WallObject, new Vector3(j - MapWidth / 2, i - MapHeight / 2, 0), Quaternion.identity);
                }
            }
        }
    }
}
