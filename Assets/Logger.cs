using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Logger : MonoBehaviour
{
    static StreamWriter logFile;

    public static FileEntry currentData = new FileEntry();

    [Serializable()]
    public class FileEntry
    {
        public int blockInteractions;

        public int aiInteractions;

        public float timeToComplete;

        public string positionData;

        public FileEntry()
        {
            positionData = "[";
        }

        public void format()
        {
            positionData = positionData.Remove(positionData.Length - 3);
            positionData += "]";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        logFile = new StreamWriter("Assets/playerlog.csv");

    }

    public static void writeData()
    {
        currentData.format();
        logFile.Write(JsonUtility.ToJson(currentData) + "\n");
        logFile.Flush();
    }

    private void OnDestroy()
    {
        logFile.Flush();
        logFile.Close();
    }
}
