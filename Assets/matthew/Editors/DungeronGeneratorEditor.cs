using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (DungeronGenerator))]
public class DungeronGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DungeronGenerator dunGen = (DungeronGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            dunGen.SpawnRooms ();
        }
    }

}
