using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UploadVoiceHandle : MonoBehaviour
{
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Uploader(1,"A","B_A","B_B","C",null,null));
    }*/
    public static IEnumerator Uploader(int pid,string partA,
        string partB_A,
        string partB_B,
        string partC,
        System.Action<ulong,float> onUploadUpdate,
        System.Action<UnityWebRequest> onUploadDone)
    {

        string url = GetPermisson.GetServerAddress + "/Grade/UploadVoice.php?token=" +
            LoginToKaoShi.userLoginCallback.data.token;

        WWWForm form = new WWWForm();
        form.AddField("pid", pid);
        form.AddField("partA",partA);
        form.AddField("partBA", partB_A);
        form.AddField("partBB", partB_B);
        form.AddField("partC", partC);

        UnityWebRequest uwr = UnityWebRequest.Post(url,form);
        uwr.SendWebRequest();
        while (true)
        {
            onUploadUpdate?.Invoke(uwr.uploadedBytes, uwr.uploadProgress);
            if (uwr.isDone) break;
            yield return new WaitForEndOfFrame();
        
        }
        onUploadDone?.Invoke(uwr);
       // Debug.Log(uwr.downloadHandler.text);
      
    }
}
