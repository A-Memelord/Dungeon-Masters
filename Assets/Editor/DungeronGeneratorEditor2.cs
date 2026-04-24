using UnityEngine;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor (typeof (DungeronGenerator2))]
public class DungeronGeneratorEditor2 : Editor
{
    public override void OnInspectorGUI()
    {
        DungeronGenerator2 dunGen = (DungeronGenerator2)target;

        DrawDefaultInspector();

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
