using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public enum eEditablePrefabs { Noone, HealthFruit, OreVein, Brawler, Scurrier }

[RequireComponent(typeof(CommandConsole))]
public class ConsoleLevelEditor : MonoBehaviour
{
    Dictionary<Room, RoomEditData> RoomEdits = new Dictionary<Room, RoomEditData>();

    [SerializeField] Texture2D mouseTexture = null;
    [SerializeField] LayerMask addMask;
    [SerializeField] LayerMask subtractMask;
    [SerializeField] GameObject AdditionReferecne = null;
    [SerializeField] GameObject SubtractReference = null;
    [SerializeField] PrefabType[] PrefabSets = null;

    GraphicRaycaster consoleRaycaster;
    EventSystem consoleEvent;
    Camera cam = null;
    CommandConsole Console = null;
    Vector3 roomOffset = Vector3.zero;
    Room currentRoom = null;
    eEditablePrefabs currentPrefab = eEditablePrefabs.Noone;

    string basePath = "Assets/_Game/Prefabs/Rooms/TEMP_LevelEdits";

    bool isAcitve = false;
    int placeMode = 1;//-1 subtract, 1 add

    #region Init
    private void Awake()
    {
        Console = GetComponent<CommandConsole>();
        consoleRaycaster = Console.ConsoleCanvas.GetComponent<GraphicRaycaster>();
        consoleEvent = Console.ConsoleCanvas.GetComponent<EventSystem>();
    }
    public void OnRevertConsole()
    {
        DeactivateEditor();
    }

    private void OnEnable()
    {
        CommandConsole.RevertConsole += OnRevertConsole;
    }
    private void OnDisable()
    {
        CommandConsole.RevertConsole -= OnRevertConsole;
    }

    #endregion


    public void EditLevel()
    {
        if (!Console.IsInEditor || !Console.IsOpen || currentRoom == null || !isAcitve)
            return;

        RaycastHit hit;
        if (!GetRayHit(out hit))
            return;

        Vector3 targetPos = -Vector3.zero;//= GetTargetPosition;
        eEditablePrefabs hitType = eEditablePrefabs.Noone;

        if (placeMode == 1)//placing an object
            targetPos = hit.point;
        else //Removing object
        {
            if (IsOfType<Scurrier>(hit) || IsOfType<Brawler>(hit))
            {
                //Deactivate the enemy only. Not adde to data
                hit.transform.gameObject.SetActive(false);
                return;
            }
            else if (IsOfType<HealthFruit>(hit))
                hitType = eEditablePrefabs.HealthFruit;
            else if (IsOfType<OreVein>(hit))
                hitType = eEditablePrefabs.OreVein;
            else
                return;

            targetPos = hit.transform.position;
        }

        if (targetPos == -Vector3.one)
            return;

        //Is this the first time to edit this room?
        if (RoomEdits == null || !RoomEdits.ContainsKey(currentRoom))
        {
            RoomEditData data = new RoomEditData(currentRoom.gameObject.name, basePath, AdditionReferecne, SubtractReference, PrefabSets);
            data.RootRoatation = currentRoom.transform.rotation; 
            RoomEdits.Add(currentRoom, data);
        }

        RecordEdit(hitType, targetPos);
        if (placeMode == 1)
            AddEditToScene(targetPos);
        else
            RemoveHitFromScene(hit);
    }

    #region Click Functionality
    private bool GetRayHit(out RaycastHit thisHit)
    {
        if (cam == null)
            cam = FindObjectOfType<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        LayerMask targetLayer;
        if (placeMode == 1)
            targetLayer = addMask;
        else targetLayer = subtractMask;

        bool retVal = Physics.Raycast(ray, out thisHit, Mathf.Infinity, targetLayer);
        return retVal;
    }

    private bool IsOfType<T>(RaycastHit hit)
    {
        return hit.transform.gameObject.GetComponent<T>() != null;
    }

    private void RecordEdit(eEditablePrefabs removedType ,Vector3 atPos)
    {
        Vector3 zeroedPos = atPos - currentRoom.transform.position;
        if (placeMode == 1)
            RoomEdits[currentRoom].MakeAddition(currentPrefab, zeroedPos);
        else
            RoomEdits[currentRoom].MakeSubtraction(removedType, zeroedPos);
    }

    private void AddEditToScene(Vector3 atPos)
    {
        GameObject targetObj = GetPrefabByType(currentPrefab);
        Transform parent = GetParentTransform(currentPrefab);
        if (targetObj == null || parent == null)
        {
            Debug.Log("Could not Spawn edit");
            return;
        }

        Debug.Log("Instantiating new object");
        GameObject newObj = Instantiate(targetObj, parent);
        newObj.transform.position = atPos;
    }

    private void RemoveHitFromScene(RaycastHit target)
    {
        target.transform.gameObject.SetActive(false);
    }
    #endregion
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void Update()
    {
        if (!isAcitve)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if(!ClickedOnConsole())
                EditLevel();
        }
    }

    public void ActivateEditor()
    {
        isAcitve = true;
        if(mouseTexture != null)
        {
            Cursor.SetCursor(mouseTexture, Vector2.zero,CursorMode.Auto);
        }

    }
    public void DeactivateEditor()
    {
        if (!isAcitve)
            return;

        isAcitve = false;
        GameManager.Instance?.RevertCursor();

    }
    public void PlayerChangedRoom(Room newRoom)
    {
        if (!Console.IsInEditor || newRoom == null || newRoom == currentRoom)
            return;

        currentRoom = newRoom;
    }

    private void OnApplicationQuit()
    {
        foreach(RoomEditData eachRoom in RoomEdits.Values)
        {
                eachRoom.SaveEditObject();
        }
    }

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Public Setters
    public void SetTargetPrefab(eEditablePrefabs target)
    {
        if (!Console.IsInEditor || !Console.IsOpen)
            return;

        currentPrefab = target;
    }
    public void TargetHealthFruit()
    {
        SetTargetPrefab(eEditablePrefabs.HealthFruit);
    }

    public void TargetOreVein()
    {
        SetTargetPrefab(eEditablePrefabs.OreVein);
    }
    public void TargetBrawler()
    {
        SetTargetPrefab(eEditablePrefabs.Brawler);
    }
    public void TargetScurrier()
    {
        SetTargetPrefab(eEditablePrefabs.Scurrier);
    }

    public void SetModeSubtract()
    {
        placeMode = -1;
    }

    public void SetModeAdd()
    {
        placeMode = 1;
    }
    #endregion 

    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Edit Data
    public struct RoomEditData
    {
        string AssetName;
        string AssetPath;

        public GameObject MasterObject;
        public Quaternion RootRoatation;
        bool masterGenerated;
        bool masterCommited;
        public Dictionary<eEditablePrefabs, List<Vector3>> Additions;
        public Dictionary<eEditablePrefabs, List<Vector3>> Subtractions;

        private GameObject AddPrefab;
        private GameObject SubPrefab;
        private PrefabType[] Prefabs;

        public void MakeAddition(eEditablePrefabs obj, Vector3 atPos)
        {
            Additions[obj].Add(atPos);
        }
        public void MakeSubtraction(eEditablePrefabs obj, Vector3 atPos)
        {
            Subtractions[obj].Add(atPos);
        }

        #region Init
        public RoomEditData(string assetName, string assetPath, GameObject addPref, GameObject subPref, PrefabType[] prefabs)
        {
            AssetName = assetName;
            AssetPath = assetPath;
            MasterObject = null;// new GameObject("EDITED_" + AssetName.ToString());

            masterCommited = false;
            masterGenerated = false;
            Additions = new Dictionary<eEditablePrefabs, List<Vector3>>(4);
            Subtractions = new Dictionary<eEditablePrefabs, List<Vector3>>(4);
            RootRoatation = Quaternion.identity;

            AddPrefab = addPref;
            SubPrefab = subPref;
            Prefabs = prefabs;

            InitDicts();
        }

        private void InitDicts()
        {
                Additions.Add(eEditablePrefabs.Brawler, new List<Vector3>());
                Additions.Add(eEditablePrefabs.Scurrier, new List<Vector3>());
                Additions.Add(eEditablePrefabs.HealthFruit, new List<Vector3>());
                Additions.Add(eEditablePrefabs.OreVein, new List<Vector3>());

                Subtractions.Add(eEditablePrefabs.Brawler, new List<Vector3>());
                Subtractions.Add(eEditablePrefabs.Scurrier, new List<Vector3>());
                Subtractions.Add(eEditablePrefabs.HealthFruit, new List<Vector3>());
                Subtractions.Add(eEditablePrefabs.OreVein, new List<Vector3>());
        }

        #endregion

        public GameObject GenerateEditObject()
        {
            if (masterGenerated) return null;
            masterGenerated = true;

            MasterObject = new GameObject("EDITED_" + AssetName);
            MasterObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Inverse(RootRoatation));

            #region Create Addtions
            GameObject addObj = new GameObject("Additions");
            addObj.transform.position = Vector3.zero;
            int totalAdditions = 0;
            foreach (List<Vector3> each in Additions.Values) totalAdditions += each.Count;

            GameObject[] addTypeObjects = new GameObject[Additions.Count];
            GameObject[] innerAdditions = new GameObject[totalAdditions];
            int i = 0; int totalJ = 0;
            foreach (eEditablePrefabs eachType in Additions.Keys)
            {
                addTypeObjects[i] = new GameObject(eachType.ToString() + "_Group");
                int count = Additions[eachType].Count;
                Vector3[] typeValues = Additions[eachType].ToArray();
                for(int j = 0; j < count; j++)
                {
                    innerAdditions[j + totalJ] = (new GameObject(eachType.ToString() + j.ToString()));
                    innerAdditions[j + totalJ].transform.position = typeValues[j];
                    innerAdditions[j + totalJ].transform.parent = addTypeObjects[i].transform;

                    GameObject visRep = Instantiate(AddPrefab);
                    visRep.transform.position = Vector3.zero;
                    visRep.transform.SetParent(innerAdditions[j + totalJ].transform, false);

                    GameObject objectPref = Instantiate(GetPrefabByType(eachType));
                    objectPref.transform.position = Vector3.zero;
                    objectPref.transform.SetParent(innerAdditions[j + totalJ].transform, false);
                }
                addTypeObjects[i].transform.SetParent(addObj.transform, false);
                i++; totalJ += count;
            }
                addObj.transform.SetParent(MasterObject.transform, false);
            #endregion

            #region Create Subtractions group
            GameObject subObj = new GameObject("Subtractions");
            subObj.transform.position = Vector3.zero;
            subObj.transform.SetParent(MasterObject.transform, false);
            int totalSubs = 0;
            foreach (List<Vector3> each in Subtractions.Values) totalSubs += each.Count;

            GameObject[] subTypeObjects = new GameObject[Subtractions.Count];
            GameObject[] innerSubs = new GameObject[totalSubs];
            int ii = 0; int totalJJ = 0;
            foreach (eEditablePrefabs eachType in Subtractions.Keys)
            {
                subTypeObjects[ii] = new GameObject(eachType.ToString() + "_Group");
                Vector3[] typeValuesS = Subtractions[eachType].ToArray();
                int count = Subtractions[eachType].Count;
                for (int j = 0; j < count; j++)
                {
                    innerSubs[j + totalJJ] = (new GameObject(eachType.ToString() + j.ToString()));
                    innerSubs[j + totalJJ].transform.position = typeValuesS[j];
                    innerSubs[j + totalJJ].transform.SetParent(subTypeObjects[ii].transform, false);
                    GameObject visRep = Instantiate(SubPrefab);
                    visRep.transform.position = Vector3.zero;
                    visRep.transform.SetParent(innerSubs[j + totalJJ].transform, false);

                }
                subTypeObjects[ii].transform.SetParent(subObj.transform, false);
                ii++; totalJJ += count;
            }
            #endregion

            return MasterObject;
        }

        private GameObject GetPrefabByType(eEditablePrefabs target)
        {
            foreach (PrefabType type in Prefabs)
                if (type.Type == target)
                    return type.Prefab;

            return null;
        }
        public void SaveEditObject()
        {
            if (masterCommited)
                return;
            masterCommited = true;
            if (!masterGenerated)
                GenerateEditObject();

            string localPath = AssetPath + "/" + "EditMask_" + MasterObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(MasterObject, localPath, InteractionMode.UserAction);
            //AssetDatabase.CreateAsset(MasterObject, AssetPath);
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Helpers
    private Transform GetParentTransform(eEditablePrefabs forType)
    {
        if(forType == eEditablePrefabs.Brawler || forType == eEditablePrefabs.Scurrier)
        {
            EnemyGroup found = currentRoom.gameObject.GetComponentInChildren<EnemyGroup>();
            if (found == null)
            {
                Debug.Log("Could not find parent for new enemy");
                return null;
            }
            return found.transform;
        }
        else
        {
            OreVein foundOre = currentRoom.gameObject.GetComponentInChildren<OreVein>();
            if (foundOre)
                return foundOre.gameObject.transform.parent;
            HealthFruit foundFruit = currentRoom.gameObject.GetComponent<HealthFruit>();
            if (foundFruit)
                return foundFruit.gameObject.transform.parent;

            Debug.Log("Could not find the parent for new pickup");
            return null;
        }
    }

    private bool ClickedOnConsole()
    {
        PointerEventData eventData = new PointerEventData(consoleEvent);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        consoleRaycaster.Raycast(eventData, results);
        if (results.Count <= 0)
            return false;
        else return true;
    }

    private GameObject GetPrefabByType(eEditablePrefabs target)
    {
        foreach (PrefabType type in PrefabSets)
            if (type.Type == target)
                return type.Prefab;

        return null;
    }
    #endregion 

    [System.Serializable]
    public class PrefabType
    {
        public eEditablePrefabs Type => _type;
        [SerializeField] eEditablePrefabs _type;

        public GameObject Prefab => _prefab;
        [SerializeField] GameObject _prefab = null;
    }
}


