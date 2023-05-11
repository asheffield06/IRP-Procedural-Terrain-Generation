using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator TerrainGen = (TerrainGenerator)target;

        if (DrawDefaultInspector())
        {
            /*if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }*/
            //TerrainGen.CreateNewMap();
        }

        if (GUILayout.Button("Generate"))
        {
            TerrainGen.CreateNewMap();
        }

    }
}
