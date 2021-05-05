using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor 
{
    [CustomEditor(typeof(RobotActionController))]
    public class TankActionControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Import Cups from file")) return;
            (target as RobotActionController)?.DrawCups();
            var asd = new List<int>();
            asd.Sort();
        }
    }
}