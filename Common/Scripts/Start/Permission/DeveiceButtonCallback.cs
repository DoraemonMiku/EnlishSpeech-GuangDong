using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveiceButtonCallback : MonoBehaviour
{
    public string deveiceName = "";
    public System.Action<string> callback;
    public void OnClick()
    {
        callback.Invoke(deveiceName);
    }
    private void OnDestroy()
    {
        callback = null;
    }
}
