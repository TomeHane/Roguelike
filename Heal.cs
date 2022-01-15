using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    //親子オブジェクトからアタッチ
    [SerializeField]
    List<ParticleSystem> healParticles;

    void Start()
    {
        //4秒後にループを切る
        StartCoroutine(LoopOff());
    }

    //ループを切るコルーチン
    IEnumerator LoopOff()
    {
        yield return new WaitForSeconds(2.0f);

        foreach (ParticleSystem particle in  healParticles)
        {
            //パーティクルの"生成"を停止
            particle.Stop();
        }

        yield return new WaitForSeconds(4.0f);

        //このオブジェクトを削除する
        Destroy(this.gameObject);
    }
}
