using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Basic Room Info")]
    [SerializeField]
    private Transform entrance;
    public Transform Entrance { get => entrance; set => entrance = value; }
    [SerializeField]
    private Transform exit;
    public Transform Exit { get => exit; set => exit = value; }
    //[HideInInspector]
    public bool overlapping = false;
    //[HideInInspector]
    public bool activeGenerating = true;
    [SerializeField]
    private Transform[] biomes = new Transform[System.Enum.GetValues(typeof(DroseraGlobalEnums.Biome)).Length];
    public Transform[] Biomes { get => biomes; set => biomes = value; }

    public RoomDataContainer data;
    [Header("Room Layouts")]
    [HideInInspector]
    [SerializeField]
    private List<Room.Layout> layouts = new List<Room.Layout>();
    [HideInInspector]
    public List<Room.Layout> Layouts { get => layouts; }

    [System.Serializable]
    public class Layout
    {
        public string name = "Layout";
        [HideInInspector]
        public bool dropdownInEditor = false;
        public int difficulty;
        public bool hidden = true; //is actually if it is shown, bad wording
        public List<Transform> objects = new List<Transform>();
    }

    /// <summary>
    /// Adds a list of objects to a layout
    /// </summary>
    /// <param name="layout"></param>
    /// <param name="objs"></param>
    public void AddObjectsToLayout(int layout, Transform[] objs)
    {
        foreach(Transform t in objs)
            if(!data.Layouts[layout].objects.Contains(t))
                data.Layouts[layout].objects.Add(t);
    }

    /// <summary>
    /// Change if a layout is active using its name (Case insensitive. Warning: will affect all layouts with the same name)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="active"></param>
    public void SetLayoutActive(string name, bool active)
    {
        foreach (Layout layout in data.Layouts)
        {
            if(layout.name.ToLower() == name.ToLower())
            {
                layout.hidden = active;
                foreach (Transform obj in layout.objects)
                {
                    obj.gameObject.SetActive(active);
                }
            }
        }
    }

    /// <summary>
    /// Change if a layout is active using its index in the layouts list
    /// </summary>
    /// <param name="index"></param>
    /// <param name="active"></param>
    public void SetLayoutActive(int index, bool active)
    {
        data.Layouts[index].hidden = active;
        foreach (Layout layout in data.Layouts)
        {
            layout.objects.RemoveAll(obj => obj == null);
            foreach (Transform obj in layout.objects)
            {
                obj.gameObject.SetActive(false);
            }
        }
        foreach (Transform obj in data.Layouts[index].objects)
        {
            obj.gameObject.SetActive(active);
        }
    }

    public void SetBiomeActive(DroseraGlobalEnums.Biome biome, bool active)
    {
        if (biomes[(int)biome] == null) return;
        foreach (Transform b in biomes)
        {
            if (b == null) continue;
            b.gameObject.SetActive(false);
        }
        biomes[(int)biome].gameObject.SetActive(active);
    }
}
