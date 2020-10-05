using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class RoomCollision : MonoBehaviour
{
    [HideInInspector]
    public Room room;

    private void Awake()
    {
        Transform current = transform;
        while(current.parent != current)
        {
            if (current.GetComponent<Room>())
            {
                room = current.GetComponent<Room>();
                break;
            }
            current = current.parent;
        }
        gameObject.layer = 14;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!room.activeGenerating) return;
        room.overlapping = true;
    }
}
