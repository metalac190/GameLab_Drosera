using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hitbox), true)]
public class HitboxEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        Hitbox hitbox = target as Hitbox;

        if(GUILayout.Button("Reset")) {
            hitbox.ResetHitbox();
        }
    }

}
