using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRainCtrl : MonoBehaviour
{
    public Transform _star,_starsParent;//流星预制体/父对象
    public int _maxStarCount = 23;//最大流星数
    public float _waitTime = 0.23f;//间隔时间
    List<Transform> starts = new List<Transform>();//已放出的流星

    float nowTime = 0f;
    private void FixedUpdate()
    {
        nowTime -= Time.deltaTime;
        for (int i = 0; i < starts.Count; i++) {
            Transform tr = starts[i];
            Vector2 pos = Camera.main.WorldToViewportPoint(tr.position);
            if (pos.x<=0f || pos.y <= 0f) {
                tr.GetComponent<Rigidbody2D>().isKinematic = true;
                
                Destroy(tr.gameObject, 1.23f);
                starts.Remove(tr);
            }
        }
        if (nowTime <= 0f&&starts.Count<_maxStarCount) {
            InsStar();
            nowTime = _waitTime;
        }
    }
    private void InsStar()
    {
     Transform tr=   Instantiate(_star, _starsParent);
     tr.position = Camera.main.ViewportToWorldPoint(new Vector3(1 + Random.Range(-0.5f, 0.5f), 1 + Random.Range(0f, 0.23f),233f));
        tr.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-100f,-6f), Random.Range(-23f, 0f));
        starts.Add(tr);
    }
}
