using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CustomLogger))]
    public class LoggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Draw voltage curve")) return;
            var curve = (target as CustomLogger)?.voltageCurve;
                
            for (int i = 0; i < curve?.keys.Length; i++)
            {
                curve.RemoveKey(i);
            }

            foreach (var keyframe in GetVoltageKeysFromFile())
            {
                curve?.AddKey(keyframe);
            }
        }

        private IEnumerable<Keyframe> GetVoltageKeysFromFile()
        {
            var sr = new StreamReader($"{Application.dataPath}\\Logs\\{(target as CustomLogger)?.filePath}.txt");
            var keyframes = new List<Keyframe>();
            while (!sr.EndOfStream)
            {
                var str = sr.ReadLine()?.Split();
                if (str != null) keyframes.Add(new Keyframe(float.Parse(str[1]), float.Parse(str[3])));
            }
            return keyframes;
        }
    }
}
