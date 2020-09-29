using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyGroup), true)]
public class EnemyGroupEditor : Editor {
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EnemyGroup group = target as EnemyGroup;

        if(GUILayout.Button("Simulate Enemy Damaged")) {
            group.OnEnemyDamage.Invoke();
        }
        if(GUILayout.Button("Simulate Attack/Ability Used")) {
            group.OnShotFired.Invoke();
        }
        if(GUILayout.Button("Simulate Grabbed Hyperseed")) {
            group.GrabHyperseed.Invoke();
        }
        if(GUILayout.Button("Turn Group Aggressive")) {
            group.TurnGroupAggressive.Invoke();
        }
        if(GUILayout.Button("Turn Group Passive")) {
            group.TurnGroupPassive.Invoke();
        }
    }

}
