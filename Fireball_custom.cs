using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fireball.csの改造版
//このスクリプトではエフェクトの制御のみ行い、火の玉を打ち出す操作は他スクリプトに任せる
public class Fireball_custom : MonoBehaviour
{
    public GameObject fieryEffect;
    public GameObject smokeEffect;
    public GameObject explodeEffect;

    Rigidbody rb;
    Collider collider;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    //何かに衝突したら
    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーか壁にぶつかったら
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Wall")
        {
            //物理演算を停止する
            rb.Sleep();
            //これまで動いていたエフェクトを消す
            if (fieryEffect != null)
            {
                Destroy(fieryEffect);
            }
            if (smokeEffect != null)
            {
                Destroy(smokeEffect);
            }
            //爆発エフェクトを動かす
            if (explodeEffect != null)
            {
                explodeEffect.SetActive(true);
            }

            //多段ヒットしないように、コライダーコンポーネントを切っておく
            collider.enabled = false;

            //5秒後にこのオブジェクトを消す
            StartCoroutine(DestroyThisObject());
        }
    }

    //このオブジェクトを消すコルーチン
    IEnumerator DestroyThisObject()
    {
        yield return new WaitForSeconds(5.0f);

        Destroy(this.gameObject);
    }


    //親オブジェクトであるFireballオブジェクトが有効になるとき
    //子オブジェクトの状態を有効[無効]にする
    public void OnEnable()
    {
        if (fieryEffect != null) {
            fieryEffect.SetActive(true);
        }
        if (smokeEffect != null)
        {
            smokeEffect.SetActive(true);
        }
        if (explodeEffect != null)
        {
            explodeEffect.SetActive(false);
        }
    }
}
