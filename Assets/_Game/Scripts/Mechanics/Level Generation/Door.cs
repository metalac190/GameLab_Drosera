using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    Text _text = null;


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

    private void Awake()
    {
        if (_text != null)
        {
            if (doorType == type.exit)
                _text.gameObject.SetActive(false);
        }
    }
}
