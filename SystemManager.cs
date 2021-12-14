/*
 *  【基本的なマップ生成の流れ】
 *  ①グリッドで壁オブジェクトを作るところ・作らないところを決めていく
 *  ②壁オブジェクトを生成
 */

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


    //道の集合点を増やしたいならこれを増やす
    [SerializeField]
    const int meetPointCount = 1;

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


    //Wallオブジェクトを配置しない箇所を決定する
    private void CreateSpaceData()
    {
        //roomCount(部屋の数)をランダムで決める
        int roomCount = Random.Range(RoomCountMin, RoomCountMax);


        //meetPointCount(道の集合点の数)ぶん要素を持つint配列を用意する
        int[] meetPointsX = new int[meetPointCount];
        int[] meetPointsY = new int[meetPointCount];

        //meetPointCount(道の集合点の数)ぶん、for文を回す
        for (int i = 0; i < meetPointCount; i++)
        {
            //道の集合点の座標をランダムで決める
            //範囲はマップ全体を16分割したときの真ん中４エリア
            meetPointsX[i] = Random.Range(MapWidth / 4, MapWidth * 3 / 4);
            meetPointsY[i] = Random.Range(MapHeight / 4, MapHeight * 3 / 4);

            //道の集合点は当然"road"マス
            Map[meetPointsY[i], meetPointsX[i]] = road;
        }


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


            //部屋が"road"と重なっていない場合
            if (isRoad == false)
            {
                //道を作成する
                //meetPointは乱数で決める(0以上meetPointCount未満)
                CreateRoadData(roadStartPointX, roadStartPointY, meetPointsX[Random.Range(0, meetPointCount)], meetPointsY[Random.Range(0, meetPointCount)]);
            }
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
                    //部屋が重なってる場合、もうCreateRoadData()は動かさなくてよい
                    //→isRoadをtrueにする
                    beCreateRoad = true;
                }
                else
                {
                    //該当マスを"road"にする
                    //∵部屋は全て"road"でもある(マスは全てwallかroad)
                    Map[roomPointY + i, roomPointX + j] = road;
                }
            }
        }
        return beCreateRoad;
    }


    // 道データを生成
    /// <param name="roadStartPointX"></param>
    /// <param name="roadStartPointY"></param>
    /// <param name="meetPointX"></param>
    /// <param name="meetPointY"></param>
    private void CreateRoadData(int roadStartPointX, int roadStartPointY, int meetPointX, int meetPointY)
    {
        //道の始点が、集合点よりも右か？
        bool isRight;
        if (roadStartPointX > meetPointX)
        {
            isRight = true;
        }
        else
        {
            isRight = false;
        }

        //道の始点が、集合点よりも下か？
        bool isUnder;
        if (roadStartPointY > meetPointY)
        {
            isUnder = false;
        }
        else
        {
            isUnder = true;
        }

        //二分の一の確率で、道をどの方向から作るかを決める
        //X方向から道を作る場合
        if (Random.Range(0, 2) == 0)
        {
            //道の始点と集合点のX座標が等しくなければ
            while (roadStartPointX != meetPointX)
            {
                //"現在の"道の始点をroadマスにする
                Map[roadStartPointY, roadStartPointX] = road;

                //道の始点が集合点よりも右にある場合
                if (isRight == true)
                {
                    //道のX座標を-1する
                    roadStartPointX--;
                }
                //道の始点が集合点よりも左にある場合
                else
                {
                    //道のX座標を+1する
                    roadStartPointX++;
                }

            }

            //道の始点と集合点のY座標が等しくなければ
            while (roadStartPointY != meetPointY)
            {
                //"現在の"道の始点をroadマスにする
                Map[roadStartPointY, roadStartPointX] = road;

                //道の始点が集合点よりも下にある場合
                if (isUnder == true)
                {
                    //道のY座標を+1する
                    roadStartPointY++;
                }
                //道の始点が集合点よりも上にある場合
                else
                {
                    //道のY座標を-1する
                    roadStartPointY--;
                }
            }

        }
        //Y方向から道を作る場合
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
