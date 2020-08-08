using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryInfoItemCtrl : MonoBehaviour
{
    public Text mainText;
    public UserMemoryList.Common common;
    public int memoryID = -1;



    public void ClickToRun()
    {
        MemoryListLoader.memoryListLoader.OpenWindowsToRead(this);
    }
    
}
