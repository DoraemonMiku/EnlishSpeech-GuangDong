using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogLoading : MonoBehaviour
{
    public float speed=360f;
    public RectTransform centerRect, midRect, outRect;


    /// <summary>
    /// 销毁这个
    /// </summary>
   public void DestoryThisLoad()
    {
        Destroy(gameObject);
    }


    private void Update()
    {
        centerRect.Rotate( Mathf.Abs(Mathf.Sin(Time.time)) *speed*Vector3.back * Time.deltaTime);
        midRect.Rotate(Mathf.Abs(Mathf.Cos(Time.time))*speed * Vector3.forward * Time.deltaTime);
        outRect.Rotate(Mathf.Abs(Mathf.Sin(Time.time))*speed * Vector3.back * Time.deltaTime);
    }
   
}
