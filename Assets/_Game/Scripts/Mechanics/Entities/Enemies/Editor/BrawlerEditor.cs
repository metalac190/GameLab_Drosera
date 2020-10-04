using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Brawler), true)]
public class BrawlerEditor : EnemyBaseEditor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        Brawler enemy = target as Brawler;

        if(GUILayout.Button("Add Waypoint")) {
            enemy.AddWaypoint();
        }
        if(GUILayout.Button("Fetch Waypoints")) {
            enemy.GetWaypoints();
        }
        if(GUILayout.Button("Clear Waypoints")) {
            enemy.ClearWaypoints();
        }
    }

}
