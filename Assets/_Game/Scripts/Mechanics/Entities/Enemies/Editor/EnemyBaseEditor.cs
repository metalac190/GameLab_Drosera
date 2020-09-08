using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyBase), true)]
public class EnemyBaseEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EnemyBase enemy = target as EnemyBase;

        if(GUILayout.Button("Idle")) {
            enemy.ForceIdle();
        }
        if(GUILayout.Button("Turn Aggressive")) {
            enemy.TurnAggressiveWrapper();
        }
        if(GUILayout.Button("Reset")) {
            enemy.ResetEnemy();
        }
    }

}
