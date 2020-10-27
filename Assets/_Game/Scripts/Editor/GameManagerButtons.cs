using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager), true)]

public class GameManagerButtons : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(GUILayout.Button("Toggle Toon Shader Rays")) {
            ToonHelper.toggleRaycasts = !ToonHelper.toggleRaycasts;
        }
        if (GUILayout.Button("Initialize All Lighting"))
        {
            ToonHelper.InitializeAllLighting();
        }
        if (GUILayout.Button("Update All Lighting"))
        {
            ToonHelper.UpdateAllLighting();
        }
        EditorGUILayout.HelpBox("You must initialize lighting if ToonHelper scripts are added. You must update lighting if objects are moved or their materials are changed.", MessageType.Info);
    }

}