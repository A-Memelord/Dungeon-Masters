using UnityEngine;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor (typeof (DungeronGenerator1))]
public class DungeronGeneratorEditor1 : Editor
{
    public override void OnInspectorGUI()
    {
        DungeronGenerator1 dunGen = (DungeronGenerator1)target;

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
