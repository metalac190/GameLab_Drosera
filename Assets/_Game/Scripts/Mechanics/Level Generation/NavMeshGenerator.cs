using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour {

    public float sizeBuffer;
    private List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
    private NavMeshModifierVolume modifier;

    private void Awake() {
        foreach(NavMeshSurface surface in GetComponentsInChildren<NavMeshSurface>())
            surfaces.Add(surface);
        modifier = GetComponentInChildren<NavMeshModifierVolume>();
    }

    public void BuildNavMesh() {
        BuildNavMesh(sizeBuffer);
    }

    public void BuildNavMesh(float sizeBuffer) {
        Vector3 radius = transform.position * 0.5f;
        radius.y = surfaces[0].center.y;
        //Debug.Log(radius + " // " + transform.InverseTransformPoint(radius));
        radius = transform.InverseTransformPoint(radius);

        float radiusLength = Mathf.Clamp(radius.magnitude * 2 * sizeBuffer, 600, 1000);
        foreach(NavMeshSurface surface in surfaces) {
            surface.center = radius;
            surface.size = new Vector3(radiusLength, surface.size.y, radiusLength);
        }

        radius.y = modifier.center.y;
        modifier.center = radius;
        modifier.size = new Vector3(radiusLength, modifier.size.y, radiusLength);

        foreach(NavMeshSurface surface in surfaces)
            surface.BuildNavMesh();
    }

}