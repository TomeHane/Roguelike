using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateObjectsManager : MonoBehaviour
{
    int frameCount = 0;

    //CreateObjects→Mazeにシーン遷移するだけ
    //Start()とUpdate()の１フレーム目が全て動ききった後に実行
    void Update()
    {
        frameCount++;

        //2フレーム目なら
        if (frameCount == 2)
        {
            SceneManager.LoadScene("Maze");
        }
    }
}
