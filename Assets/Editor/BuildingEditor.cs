using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingEditor : Editor {

    public override void OnInspectorGUI() {
        BuildingGenerator building = target as BuildingGenerator;

        if (DrawDefaultInspector()) {
            building.GenerateBuilding();
        }
        if (GUILayout.Button("Generate Building")) {
            building.GenerateBuilding();
        }

    }

}
