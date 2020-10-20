using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager), true)]

public class GameManagerButtons : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(GUILayout.Button("Toggle Toon Shader Rays")) {
            AwesomeToon.AwesomeToonHelper.toggleRaycasts = !AwesomeToon.AwesomeToonHelper.toggleRaycasts;
        }
    }

}