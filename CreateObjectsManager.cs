using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateObjectsManager : MonoBehaviour
{
    int frameCount = 0;

    //CreateObjects��Maze�ɃV�[���J�ڂ��邾��
    //Start()��Update()�̂P�t���[���ڂ��S�ē�����������Ɏ��s
    void Update()
    {
        frameCount++;

        //2�t���[���ڂȂ�
        if (frameCount == 2)
        {
            SceneManager.LoadScene("Maze");
        }
    }
}
