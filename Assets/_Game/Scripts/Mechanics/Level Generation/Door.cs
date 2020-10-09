using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    enum type { entrance, exit };
    [HideInInspector]
    public Room room;
    [SerializeField]
    Mesh arrowGizmo;
    [SerializeField]
    type doorType = type.entrance;

    private void OnTriggerEnter(Collider other)
    {
        if(doorType == type.entrance)
        {
            // TODO
        }
        if(doorType == type.exit)
        {
            // TODO
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(arrowGizmo, transform.position, transform.rotation);
    }
}
