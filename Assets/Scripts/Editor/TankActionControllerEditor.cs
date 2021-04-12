using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TankActionController))]
    public class TankActionControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Import Cups from file")) return;
            (target as TankActionController)?.DrawCups();
            var asd = new List<int>();
            asd.Sort();
        }
    }
}
