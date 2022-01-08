using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyManager : MonoBehaviour
{
    public List<GameObject> DontDestroyList;

    void Start()
    {
        foreach(GameObject obj in DontDestroyList)
        {
            DontDestroyOnLoad(obj);
        }

        DontDestroyOnLoad(this.gameObject);
    }


    //このオブジェクトを含む、DontDestroyOnLoadオブジェクトをまとめて削除する関数
    public void DestroyAll()
    {


        //デバッグ
        Debug.Log("オブジェクトを削除");



        foreach (GameObject obj in DontDestroyList)
        {
            Destroy(obj);
        }

        //thisだけだとコンポーネントしか削除できないので注意！
        Destroy(this.gameObject);
    }
}
