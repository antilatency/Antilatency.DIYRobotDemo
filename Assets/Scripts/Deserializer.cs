using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Deserializer {
    public static List<Vector3> GetPositions(string path, float multiplier) {
        var cups = new List<Vector3>();
        using(var fs = new FileStream(path, FileMode.Open))
            try {
                var reader = new BinaryReader(fs);
                while(reader.BaseStream.Position != reader.BaseStream.Length){
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    cups.Add(new Vector3(x, 0, y) * multiplier) ;
                }
            }
            catch(Exception e){
                Debug.LogFormat($"Deserialization exception: {e.Message}");
            }
        return cups;
    }
}
