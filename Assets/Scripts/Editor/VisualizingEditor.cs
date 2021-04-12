using Graphics;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Visualizing))]
    public class VisualizingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Draw cups from file"))
            {
                (target as Visualizing)?.DrawCups(); 
            }
        }
    }
}
