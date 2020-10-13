using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTestScript : MonoBehaviour {

    public static List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
    public static bool built = false;

    void Awake() {
        /*foreach(NavMeshSurface comp in GameObject.FindObjectsOfType<NavMeshSurface>()) {
            comp.BuildNavMesh();
        }*/
        surfaces.Add(GetComponent<NavMeshSurface>());
    }

    private void Start() {
        if(!built) {
            foreach(NavMeshSurface surfaces in surfaces)
                surfaces.BuildNavMesh();
            built = true;
        }
    }

}