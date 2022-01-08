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


    //���̃I�u�W�F�N�g���܂ށADontDestroyOnLoad�I�u�W�F�N�g���܂Ƃ߂č폜����֐�
    public void DestroyAll()
    {


        //�f�o�b�O
        Debug.Log("�I�u�W�F�N�g���폜");



        foreach (GameObject obj in DontDestroyList)
        {
            Destroy(obj);
        }

        //this�������ƃR���|�[�l���g�����폜�ł��Ȃ��̂Œ��ӁI
        Destroy(this.gameObject);
    }
}
