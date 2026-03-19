using UnityEngine;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor (typeof (DungeronGenerator))]
public class DungeronGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DungeronGenerator dunGen = (DungeronGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate 1"))
        {
            dunGen.SpawnRoom();
        }

        if (GUILayout.Button("Generate all"))
        {
            dunGen.SpawnAllRooms();
        }

        if (GUILayout.Button("Clear Rooms"))
        {
            dunGen.ClearRooms();
        }
    }

}
