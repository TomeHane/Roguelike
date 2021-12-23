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
    //MoveEnemy.csで使うのでpublic
    public int MapWidth = 50;
    public int MapHeight = 50;

    //int型の二次元配列。要素数はMapWidthとMapHeightで決定する。
    public int[,] Map;

    int wall = 9;
    int road = 0;

    //Wallオブジェクトをアタッチ
    public GameObject WallObject;
    //床オブジェクトをアタッチ
    public GameObject groundObject;
    //壁・床の一辺の長さを入れる
    public int squareLength;


    //部屋の高さの最小・最大値
    [SerializeField]
    int roomMinHeight = 5;
    [SerializeField]
    int roomMaxHeight = 10;

    //部屋の横幅の最小・最大値
    [SerializeField]
    int roomMinWidth = 5;
    [SerializeField]
    int roomMaxWidth = 10;

    //部屋数の最小・最大値
    [SerializeField]
    int RoomCountMin = 10;
    [SerializeField]
    int RoomCountMax = 15;


    //道の集合点を増やしたいならこれを増やす
    //2以上だと道が途切れる可能性あり
    const int meetPointCount = 1;


    //スポーン状況を管理するための列挙型
    enum SpownArea
    {
        Disabled,
        Enabled,
        Player,
        EnemySkeleton,
        Enemy_piyo
    }

    //マス目ごとにスポーン状況を入れていく
    SpownArea[,] spownAreas;

    //スポーン可能エリアを覚えさせるリスト
    //エネミーが目的地を設定するときにも使うのでpublic
    //System.NonSerialized:インスペクター上に表示しない
    [System.NonSerialized]
    public List<int> spawnablePointListX = new List<int>();
    [System.NonSerialized]
    public List<int> spawnablePointListY = new List<int>();

    //オブジェクト設置エリアを覚えさせるリスト
    List<int> placedPointListX = new List<int>();
    List<int> placedPointListY = new List<int>();

    //設置するオブジェクト
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

    //Mapの二次元配列の初期化
    //spownAreasの二次元配列の初期化
    private void ResetMapData()
    {
        Map = new int[MapHeight, MapWidth];
        spownAreas = new SpownArea[MapHeight, MapWidth];

        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                //wallはint値
                Map[i, j] = wall;

                //全マスのスポーン状況を"使用不可"に
                spownAreas[i, j] = SpownArea.Disabled;
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
            int roomPointY = Random.Range(2, MapHeight - roomMaxHeight - 2);

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
                    //マップサイズが50 * 50の場合、2500個のオブジェクトを生成する
                    //【Wallの一辺の長さ】mずつ配置されていく。
                    Instantiate(WallObject, new Vector3(squareLength * (j - MapWidth / 2), 0, squareLength * (i - MapHeight / 2)), Quaternion.identity);
                }
                //Map[i, j]がroadなら
                else
                {
                    Instantiate(groundObject, new Vector3(squareLength * (j - MapWidth / 2), 0, squareLength * (i - MapHeight / 2)), Quaternion.identity);
                }
            }
        }
    }


    //スポーン可能エリアをチェックする関数
    void CheckSpownArea()
    {
        //周囲八方がroadである必要があるため、壁際２マスはチェック対象外
        for(int i = 2; i < MapHeight - 2; i++)
        {
            for (int j = 2; j < MapWidth -2; j++)
            {
                //該当マスがroadなら
                if (Map[i, j] == road)
                {
                    //一旦スポーン可能マスにする
                    spownAreas[i, j] = SpownArea.Enabled;

                    //リスト先頭にスポーン可能エリアを覚えさせる
                    spawnablePointListX.Insert(0, j);
                    spawnablePointListY.Insert(0, i);

                    //周囲八方を確認する関数
                    CheckAllSides(i, j);
                }
            }
        }
    }

    //周囲八方を確認する関数
    void CheckAllSides(int i, int j)
    {
        for (int k = -1; k <= 1; k++)
        {
            for (int l = -1; l <= 1; l++)
            {
                //もし八方のいずれかがroadじゃなければ
                if(Map[i + k, j + l] != road)
                {
                    //スポーン不可マスにする
                    //∵"通路"ではなく"部屋"でのみスポーンさせたいから
                    spownAreas[i, j] = SpownArea.Disabled;

                    //リスト先頭の値を削除する
                    spawnablePointListX.RemoveAt(0);
                    spawnablePointListY.RemoveAt(0);

                    return;
                }
            }
        }
    }


    //キャラクター等のオブジェクト設置する関数
    void PlacedObjects()
    {
        //プレイヤーの設置位置を決定する
        DecideObjectPosition(SpownArea.Player);

        //スケルトンエネミーの数を決定する
        int enemySkeletonNum = Random.Range(2, 5);
        for (int i = 0; i < enemySkeletonNum; i++)
        {
            //スケルトンエネミーの設置位置を決定する
            DecideObjectPosition(SpownArea.EnemySkeleton);
        }



        //オブジェクトの設置
        for (int i = 0; i < placedPointListX.Count; i++)
        {
            int x = placedPointListX[i];
            int y = placedPointListY[i];

            //プレイヤーの設置
            if (spownAreas[y, x] == SpownArea.Player)
            {
                playerObject.transform.position = new Vector3(squareLength * (x - MapWidth / 2), 0, squareLength * (y - MapHeight / 2));
                playerObject.SetActive(true);
            }
            //スケルトンエネミーの生成
            if (spownAreas[y, x] == SpownArea.EnemySkeleton)
            {
                Instantiate(enemySkeleton, new Vector3(squareLength * (x - MapWidth / 2), 0, squareLength * (y - MapHeight / 2)), Quaternion.identity);
            }
        }
    }

    //オブジェクトの設置位置を決める関数
    void DecideObjectPosition(SpownArea spownArea)
    {
        //オブジェクトの設置位置をランダムで決定する
        int randomNum = Random.Range(0, spawnablePointListX.Count);
        int objectPointX = spawnablePointListX[randomNum];
        int objectPointY = spawnablePointListY[randomNum];

        //オブジェクトの設置位置のスポーン状況を【対応するもの】に変更
        spownAreas[objectPointY, objectPointX] = spownArea;

        //スポーン状況が【Enabled】ではなくなったので、SpawnablePointListから削除
        spawnablePointListX.RemoveAt(randomNum);
        spawnablePointListY.RemoveAt(randomNum);

        //placedPointListX[Y]にオブジェクト設置エリアを覚えさせる
        //実質的に、spawnablePointListX[Y] → placedPointListX[Y]への移し替えを行っている
        placedPointListX.Insert(0, objectPointX);
        placedPointListY.Insert(0, objectPointY);
    }
}
