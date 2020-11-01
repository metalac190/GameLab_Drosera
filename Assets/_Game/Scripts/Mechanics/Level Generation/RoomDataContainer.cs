using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataContainer : MonoBehaviour
{
    [Header("Room Layouts")]
    [SerializeField]
    private List<Room.Layout> layouts = new List<Room.Layout>();
    public List<Room.Layout> Layouts { get => layouts; }
}
