using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomWindow : EditorWindow
{
    Room currentRoom = null;
    int selected = 0;
    Vector2 scrollPosition;
    int currentHeight;
    int widthBuffer = 5;
    int heightBuffer = 5;
    bool biomeDropdown = false;
    bool doorDropdown = false;
    int prevDifficulty = -1000;
    
    string[] biomes = { "Jungle" , "Desert"};
    Color sectionColor = new Color(.7f, .7f, .7f);
    Color subsectionColor = new Color(.8f, .8f, .8f);

    [MenuItem("Tools/Room Editor")]
    public static void DrawWindow()
    {
        GetWindow<RoomWindow>("Room Editor");
    }

    private void Awake()
    {
        minSize = new Vector2(520 + widthBuffer*2, 200 + heightBuffer*3);

        selected = Selection.transforms.Length;

        if (selected > 0 && Selection.transforms[0].root.GetComponent<Room>())
        {
            currentRoom = Selection.transforms[0].root.GetComponent<Room>();
        }

        Repaint();
    }

    private void OnSelectionChange()
    {
        selected = Selection.transforms.Length;

        if (selected > 0 && Selection.transforms[0].root.GetComponent<Room>())
        {
            currentRoom = Selection.transforms[0].root.GetComponent<Room>();
        }
        
        Repaint();
    }

    private void OnGUI()
    {
        DrawRoomEditor();
    }

    /// <summary>
    /// Draws the overall editor window
    /// </summary>
    private void DrawRoomEditor()
    {
        if (currentRoom != null)
        {
            if(currentRoom.data == null)
            {
                currentRoom.data = currentRoom.GetComponentInChildren<RoomDataContainer>();
                if(currentRoom.data != null)
                {
                    currentRoom.data.Layouts.AddRange(currentRoom.Layouts);
                    EditorUtility.SetDirty(currentRoom.data.gameObject);
                }
            }

            currentHeight = 60;
            if (biomeDropdown) currentHeight += System.Enum.GetValues(typeof(DroseraGlobalEnums.Biome)).Length * 20;
            if (doorDropdown) currentHeight += 40;
            DrawRoomSettings();
            DrawLayoutSettings();
        }
    }

    /// <summary>
    /// Draws the room settings area
    /// </summary>
    private void DrawRoomSettings()
    {
        GUILayout.BeginArea(new Rect(widthBuffer, heightBuffer, position.width - widthBuffer * 2, currentHeight + heightBuffer));
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, sectionColor);
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, position.width, currentHeight), texture);
        
        GUILayout.Box(currentRoom.name + " Room Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        doorDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(doorDropdown, "Door Transforms");

        if (doorDropdown)
        {
            if (GUILayout.Button("Entrance: " + (currentRoom.Entrance == null ? "Not Set" : currentRoom.Entrance.name)))
            {
                if (selected == 1 && Selection.transforms[0].GetComponent<Door>())
                {
                    currentRoom.Entrance = Selection.transforms[0];
                    currentRoom.Entrance.GetComponent<Door>().room = currentRoom;
                    EditorUtility.SetDirty(currentRoom.gameObject);
                }

            }
            if (GUILayout.Button("Exit: " + (currentRoom.Exit == null ? "Not Set" : currentRoom.Exit.name)))
            {
                if (selected == 1 && Selection.transforms[0].GetComponent<Door>())
                {
                    currentRoom.Exit = Selection.transforms[0];
                    currentRoom.Exit.GetComponent<Door>().room = currentRoom;
                    EditorUtility.SetDirty(currentRoom.gameObject);
                }

            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        biomeDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(biomeDropdown, "Biome Containers");

        if (biomeDropdown)
        {
            foreach (DroseraGlobalEnums.Biome biome in System.Enum.GetValues(typeof(DroseraGlobalEnums.Biome)))
            {
                if (GUILayout.Button(biome + ": " + (currentRoom.Biomes[(int)biome] != null ? currentRoom.Biomes[(int)biome].name : "Not Set")))
                {
                    if (selected == 1)
                    {
                        currentRoom.Biomes[(int)biome] = Selection.transforms[0];
                        EditorUtility.SetDirty(currentRoom.gameObject);
                    }
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws the overall layout settings and layouts area
    /// </summary>
    private void DrawLayoutSettings()
    {
        GUILayout.BeginArea(new Rect(widthBuffer, currentHeight + heightBuffer * 2, position.width - widthBuffer * 2, position.height + heightBuffer * 2));
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, sectionColor);
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height - currentHeight - heightBuffer*3), texture);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("Layouts", EditorStyles.boldLabel);
        if (GUILayout.Button("Add New Layout"))
        {
            currentRoom.data?.Layouts.Add(new Room.Layout());
            EditorUtility.SetDirty(currentRoom.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width - widthBuffer*2), GUILayout.Height(position.height - currentHeight - heightBuffer*7));
        if (currentRoom.data != null)
            foreach (Room.Layout layout in currentRoom.data.Layouts)
            {
                if (DrawLayout(layout)) break;
                EditorGUILayout.Space(4);
            }
        else EditorGUILayout.HelpBox("Please add a RoomDataContainer script to the collision prefab, then hover over this window again!", MessageType.Error);
        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    /// <summary>
    /// Draws everything needed for an individual layout
    /// </summary>
    /// <param name="layout"></param>
    /// <returns> break out of loop? </returns>
    private bool DrawLayout(Room.Layout layout)
    {
        /*Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, subsectionColor);
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height - settingsHeight - heightBuffer * 3), texture);*/

        EditorGUILayout.BeginHorizontal();
        layout.dropdownInEditor = EditorGUILayout.BeginFoldoutHeaderGroup(layout.dropdownInEditor, ""+layout.objects.Count);

        EditorGUI.BeginChangeCheck();
        layout.name = EditorGUILayout.TextField(layout.name);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(currentRoom.data.gameObject);
        }

        if (GUILayout.Button("Add Selected Objects to Layout"))
        {
            currentRoom.AddObjectsToLayout(currentRoom.data.Layouts.IndexOf(layout), Selection.transforms);
            EditorUtility.SetDirty(currentRoom.data.gameObject);
        }

        if (GUILayout.Button(layout.hidden ? "Shown" : "Hidden"))
        {
            layout.hidden = !layout.hidden;
            foreach (Transform obj in layout.objects)
            {
                obj.gameObject.SetActive(layout.hidden);
                EditorUtility.SetDirty(currentRoom.data.gameObject);
            }
        }

        if (GUILayout.Button("Select All"))
        {
            Object[] objs = new Object[layout.objects.Count];
            for (int i = 0; i < layout.objects.Count; i++)
            {
                objs[i] = layout.objects[i].gameObject;
            }
            Selection.objects = objs;
        }

        if (GUILayout.Button("Delete Layout"))
        {
            currentRoom.data.Layouts.Remove(layout);
            EditorUtility.SetDirty(currentRoom.data.gameObject);
            return true;
        }

        EditorGUILayout.EndHorizontal();

        if (layout.dropdownInEditor)
        {
            EditorGUI.BeginChangeCheck();
            layout.difficulty = EditorGUILayout.IntSlider("Difficulty", layout.difficulty, -3, 15);
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(currentRoom.data.gameObject);
            }

            List<Transform> toRemove = new List<Transform>();
            foreach (Transform obj in layout.objects)
            {
                if (obj == null)
                {
                    toRemove.Add(obj);
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Box(obj.name, GUILayout.Width(100));
                if (GUILayout.Button("Select"))
                {
                    Object[] selected = { obj.gameObject };
                    Selection.objects = selected;
                }
                if (GUILayout.Button("Remove"))
                {
                    toRemove.Add(obj);
                    EditorUtility.SetDirty(currentRoom.data.gameObject);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
            }
            if (toRemove.Count > 0)
            {
                layout.objects.RemoveAll(obj => toRemove.Contains(obj));
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        return false;
    }
}
