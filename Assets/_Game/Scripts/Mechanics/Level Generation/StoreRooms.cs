using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreRooms : MonoBehaviour
{
    [Header("Store Room Prefabs")]
    [Tooltip("Drag in all available rooms")]
    [SerializeField]
    private List<GameObject> allRooms = new List<GameObject>();
    public List<GameObject> AllRooms { get => allRooms; }
}
