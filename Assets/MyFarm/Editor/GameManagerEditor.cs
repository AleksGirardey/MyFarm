using MyFarm.Scripts.GameManager;
using UnityEditor;
using UnityEngine;

namespace MyFarm.Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Reset Player Data"))
                PlayerPrefs.DeleteAll();
            base.OnInspectorGUI();
        }
    }
}