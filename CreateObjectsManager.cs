using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateObjectsManager : MonoBehaviour
{
    //CreateObjects��Maze�ɃV�[���J�ڂ��邾��
    //Start()���S�ē�����������Ɏ��s����������Update()���g�p
    void Update()
    {
        SceneManager.LoadScene("Maze");
    }
}
