using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//カメラのViewPortRectを調整するスクリプト
public class AspectController : MonoBehaviour
{
    public float x_aspect = 16.0f;
    public float y_aspect = 9.0f;

    [SerializeField]
    Camera cam;

    //最新のアスペクト比
    float latestWindowAspect;


    void Awake()
    {
        //Rectを更新する
        Rect rect = calcAspect(x_aspect, y_aspect);
        cam.rect = rect;

        //Update用に初期化しておく
        latestWindowAspect = (float)Screen.width / (float)Screen.height;
    }


    public void Update()
    {
        //現在のアスペクト比を取得
        float currentWindowAspect = (float)Screen.width / (float)Screen.height;

        //アスペクト比が変わっていたら
        if (currentWindowAspect != latestWindowAspect)
        {
            //Rectを変更する
            Rect rect = calcAspect(x_aspect, y_aspect);
            cam.rect = rect;

            //最新のアスペクト比を更新
            latestWindowAspect = currentWindowAspect;
        }
    }


    private Rect calcAspect(float width, float height)
    {
        float target_aspect = width / height;
        float window_aspect = (float)Screen.width / (float)Screen.height;
        float scale_height = window_aspect / target_aspect;
        Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (1.0f > scale_height)
        {
            rect.x = 0;
            rect.y = (1.0f - scale_height) / 2.0f;
            rect.width = 1.0f;
            rect.height = scale_height;
        }
        else
        {
            float scale_width = 1.0f / scale_height;
            rect.x = (1.0f - scale_width) / 2.0f;
            rect.y = 0.0f;
            rect.width = scale_width;
            rect.height = 1.0f;
        }
        return rect;
    }
}
