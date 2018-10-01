using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace StarMap
{
    [CustomEditor(typeof(StarData))]
    public class StarDataEditor : Editor
    {
        public float magnitudeLimit = 7;

        public override void OnInspectorGUI()
        {
            StarData data = target as StarData;

            magnitudeLimit = EditorGUILayout.FloatField("Magnitude Limit", magnitudeLimit);

            if (GUILayout.Button("Generate Data"))
            {
                data.LoadFromDatabase(magnitudeLimit);
            }

            if (data.stars == null)
            {
                GUILayout.Label("No stars");
            }
            else
            {
                string count = data.stars.Length.ToString();
                EditorGUILayout.LabelField("Stars loaded: " + count);

                EditorUtility.SetDirty(data);
            }
        }
    }
}