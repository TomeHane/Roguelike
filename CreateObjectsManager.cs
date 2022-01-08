using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateObjectsManager : MonoBehaviour
{
    //CreateObjects→Mazeにシーン遷移するだけ
    //Start()が全て動ききった後に実行したいからUpdate()を使用
    void Update()
    {
        SceneManager.LoadScene("Maze");
    }
}
