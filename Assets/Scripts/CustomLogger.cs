using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CustomLogger : MonoBehaviour
{

    public AnimationCurve voltageCurve;

    public string filePath;


    private StreamWriter _streamWriter;
    

    private void Awake()
    {
        var path = $"{Application.dataPath}\\Logs\\{filePath}.txt";
        Debug.LogWarning(path);
        var fileStream = new FileStream(path, 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.None);
        _streamWriter = new StreamWriter(fileStream);
    }

    private void OnApplicationQuit()=>
        _streamWriter.Close();
    

    public void WriteLogInfoLine(string info)=>
        _streamWriter.WriteLine(info);
    
}
