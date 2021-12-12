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


    //部屋の高さの最小・最大値
    [SerializeField]
    const int roomMinHeight = 5;
    [SerializeField]
    const int roomMaxHeight = 10;

    //部屋の横幅の最小・最大値
    [SerializeField]
    const int roomMinWidth = 5;
    [SerializeField]
    const int roomMaxWidth = 10;

    //部屋数の最小・最大値
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


    private void CreateSpaceData()
    {
        //roomCount(部屋の数)をランダムで決める
        int roomCount = Random.Range(RoomCountMin, RoomCountMax);

        //部屋数分、処理を行う
        for (int i = 0; i < roomCount; i++)
        {
            //部屋の高さ、横幅をランダムで決める
            int roomHeight = Random.Range(roomMinHeight, roomMaxHeight);
            int roomWidth = Random.Range(roomMinWidth, roomMaxWidth);

            //部屋の作成地点(左下)を決める
            //壁の端から必ず2m離れるように調整
            //マップサイズが50 * 50の場合、[1～50, 1～50]を調整した値になる
            int roomPointX = Random.Range(2, MapWidth - roomMaxWidth - 2);
            int roomPointY = Random.Range(2, MapWidth - roomMaxWidth - 2);

            //道の開始地点を、部屋のグリッドの範囲からランダムに決める
            int roadStartPointX = Random.Range(roomPointX, roomPointX + roomWidth);
            int roadStartPointY = Random.Range(roomPointY, roomPointY + roomHeight);

            //これまでの情報をもとに、部屋を作成する
            //既に作った部屋と重なった場合、trueが返ってくる
            bool isRoad = CreateRoomData(roomHeight, roomWidth, roomPointX, roomPointY);
        }
    }


    // 部屋データを生成。すでに部屋がある場合はtrueを返し、道を作らないようにする
    /// <param name="roomHeight">部屋の高さ</param>
    /// <param name="roomWidth">部屋の横幅</param>
    /// <param name="roomPointX">部屋の始点(x)</param>
    /// <param name="roomPointY">部屋の始点(y)</param>
    /// <returns></returns>
    private bool CreateRoomData(int roomHeight, int roomWidth, int roomPointX, int roomPointY)
    {
        bool beCreateRoad = false;

        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomWidth; j++)
            {
                //該当マスが既に"road"だった場合
                if (Map[roomPointY + i, roomPointX + j] == road)
                {
                    //戻り値をtrueにする
                    beCreateRoad = true;
                }
                else
                {
                    //該当マスを"road"にする
                    Map[roomPointY + i, roomPointX + j] = road;
                }
            }
        }
        return beCreateRoad;
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
                    //マップサイズが50 * 50の場合、(-25～24, -25～24, 0)の座標にWallオブジェクトを生成している
                    //j, iが1ずつ変化していくので、Wallも1mずつ配置されていく。
                    Instantiate(WallObject, new Vector3(j - MapWidth / 2, i - MapHeight / 2, 0), Quaternion.identity);
                }
            }
        }
    }
}
