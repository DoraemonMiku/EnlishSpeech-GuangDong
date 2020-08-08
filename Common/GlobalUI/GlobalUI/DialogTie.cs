using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogTie : MonoBehaviour
{
    public RectTransform myRect;
    public Text myText;
    void Start()
    {
        myRect.DOLocalMoveY(0f, 0.23f);
        Destroy(gameObject, 3.4f);
    }
    public void SetText(string str)
    {
        myText.text = str;
    }
}
