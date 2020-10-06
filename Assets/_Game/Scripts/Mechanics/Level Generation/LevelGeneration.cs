using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField]
    private int levelNumber = 0;
    public int LevelNumber { get => levelNumber; }

    [Header("Master Room Prefab")]
    [SerializeField]
    private GameObject roomMasterPrefab;
    public GameObject RoomMasterPrefab { get => roomMasterPrefab; }
    private List<GameObject> roomMasterList = new List<GameObject>();
    private GameObject playerObject;        //AUTOMATICALLY SEARCHES FOR OBJECT WITH "Player" TAG
    [Header("Difficulty Scale Ratio")]
    [SerializeField]
    private float levelScaleRatio;
    public float LevelScaleRatio { get => levelScaleRatio; }
    [Header("Level Biomes")]
    [SerializeField]
    private List<DroseraGlobalEnums.Biome> levelBiomesList = new List<DroseraGlobalEnums.Biome>();
    public List<DroseraGlobalEnums.Biome> LevelBiomesList { get => levelBiomesList; }

    [Header("Level Information")]
    [SerializeField]
    private float baseDifficulty;
    public float BaseDifficulty { get => baseDifficulty; set => baseDifficulty = value; }
    [SerializeField]
    private float desiredLevelDifficulty;
    public float DesiredLevelDifficulty { get => desiredLevelDifficulty; set => desiredLevelDifficulty = value; }
    [SerializeField]
    [Tooltip("The difficulty at end of level. DO NOT EDIT")]
    private float currentLevelDifficulty = 0;
    public float CurrentLevelDifficulty { get => currentLevelDifficulty; set => currentLevelDifficulty = value; }
    [SerializeField]
    private GameObject dropShipRoom;
    public GameObject DropShipRoom { get => dropShipRoom; set => dropShipRoom = value; }
    [SerializeField]
    private GameObject endRoom;
    public GameObject EndRoom { get => endRoom; set => endRoom = value; }

    //Variables to detect Entrance/Exit Rotations
    private Vector3 currentExitLocation;
    private float priorRoomExitRotation = 0;    //default should = dropship room exit rotation
    private float currentLevelRotation = 0;     //to prevent room spiraling with angular arrangements
    private Quaternion priorRoomRotation;
    private bool whileCheck = true;

    //level Generation check bool
    private bool genTest = false;

    void Start() //on scene start, generate level
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        randomizeBiomes();
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GenerateLevelTrigger();
        }
    }*/
    
    public void GenerateLevelTrigger()
    {
        StartCoroutine(GenerateLevelCoroutine());
    }
    IEnumerator GenerateLevelCoroutine()
    {
        genTest = false;
        if (levelNumber < 6)
        {
            levelNumber += 1;
            while (genTest == false)
            {
                StartCoroutine(CreateLevelCoroutine(levelNumber, roomMasterPrefab.GetComponent<StoreRooms>().AllRooms));
                yield return new WaitForSeconds(.02f);
            }

        }
        else
        {
            Debug.Log("At Max Level.");
        }
        yield return null;
    }

    private void ShuffleRoomList(List<GameObject> roomList)         //will shuffle room array
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject placeHolder = roomList[i];
            int randomNum = Random.Range(i, roomList.Count); //may change i to 0?
            roomList[i] = roomList[randomNum];
            roomList[randomNum] = placeHolder;
        }
    }

    /// <summary>
    /// This fucntion will take in the the randomized roomList and instantiate the first room if it passes check.
    /// Room is removed from list when instantiated.
    /// </summary>
    private bool InstantiateValidRoom(List<GameObject> roomList)
    {
        GameObject toRemove = null;
        bool roomCheck = true;
        float blank = 0;
        for (int i = 0; i < roomList.Count; i++) //goes through list until a break (will break on chosen room/continues on failed room check)
        {
            if (blank == 0)
            {        //will be room rotation check
                toRemove = roomList[i];
                GameObject plz = Instantiate(roomList[i], currentExitLocation + FixTransformDeficit(roomList[i]), priorRoomRotation);
                plz.transform.RotateAround(currentExitLocation, Vector3.up, RotateRoom(priorRoomRotation, roomList[i]));
                //priorRoomRotation = plz.transform.rotation;
                priorRoomRotation = plz.GetComponent<Room>().Exit.transform.rotation;
                plz.GetComponent<Room>().Entrance.transform.SetParent(null);
                plz.transform.SetParent(plz.GetComponent<Room>().Entrance, true);
                plz.GetComponent<Room>().Entrance.transform.position = currentExitLocation;
                currentExitLocation = plz.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
                Physics.autoSimulation = false;
                Physics.Simulate(0.001f);
                Physics.autoSimulation = true;

                if (plz.GetComponent<Room>().overlapping == true)
                {
                    Debug.Log(plz.name + " is overlapping a previous room");
                    //return false;
                    roomCheck = false;
                }
                else
                {
                    Debug.Log("Safe: " + plz.name);
                }
                //check if room intersect, if so regen level ?                   
                //activate layout and add difficulty (get number of avaliable layouts)  Layouts.Count  Random.Range();
                int randomLayout = Random.Range(0, plz.GetComponent<Room>().Layouts.Count);
                plz.GetComponent<Room>().SetLayoutActive(randomLayout, true);
                //activate nav mesh (find compnenets in children and bake)
                NavMeshSurface[] navComponents = plz.GetComponentsInChildren<NavMeshSurface>();
                foreach (NavMeshSurface comp in navComponents)
                {
                    comp.BuildNavMesh();
                }
                currentLevelDifficulty += plz.GetComponent<Room>().Layouts[randomLayout].difficulty;
                break;
            }
        }
        //if to Remove != null *************
        roomList.Remove(toRemove);
        if (roomList.Count == 0)
        {
            whileCheck = false; //safety check in case room runs out defor desired difficulty
        }
        return roomCheck;
    }

    private bool InstantiateEndRoom(GameObject lastRoom)
    {
        bool roomCheck = true;
        GameObject plz = Instantiate(lastRoom, currentExitLocation + FixTransformDeficit(lastRoom), priorRoomRotation);
        plz.transform.RotateAround(currentExitLocation, Vector3.up, RotateRoom(priorRoomRotation, lastRoom));
        priorRoomRotation = plz.GetComponent<Room>().Exit.transform.rotation;
        plz.GetComponent<Room>().Entrance.transform.SetParent(null);
        plz.transform.SetParent(plz.GetComponent<Room>().Entrance, true);
        plz.GetComponent<Room>().Entrance.transform.position = currentExitLocation;
        if (plz.GetComponent<Room>().overlapping == true)
        {
            Debug.Log(plz.name + " is overlapping a previous room");
            roomCheck = false;
        }
        else
        {
            Debug.Log("Safe: " + plz.name);
        }
        NavMeshSurface[] navComponents = plz.GetComponentsInChildren<NavMeshSurface>();
        foreach (NavMeshSurface comp in navComponents)
        {
            comp.BuildNavMesh();
        }
        return roomCheck;
    }

    public float ScaleDifficulty()
    {
        float returnVal = 0;
        if (levelNumber == 1)
        {
            return baseDifficulty;
        }
        returnVal = BaseDifficulty * Mathf.Pow(LevelScaleRatio, levelNumber);
        //its non updated level diff since formula would be actual level - 1 anyway.
        return returnVal;
    }

    private Vector3 FixTransformDeficit(GameObject genericRoom) {return (genericRoom.transform.position - genericRoom.GetComponent<Room>().Entrance.transform.position);}

    private float RotateRoom(Quaternion previousRoom, GameObject genericRoom)
    {
        Quaternion adjustmentRotation = new Quaternion(0, 0, 0, 0);
        float neededRotate = 0; //the y rotation needed to match

        if (genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y > 180)
        {
            neededRotate = (360 - genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y);
        }
        else if (genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y <= 180)
        {
            neededRotate = -(genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y);
        }
        return neededRotate;
    }

    IEnumerator CreateLevelCoroutine(int currentLevel, List<GameObject> masterList)
    {
        DestroyInstantiatedRooms(); //destroys previous level
        yield return new WaitForEndOfFrame();
        Physics.autoSimulation = false;
        Physics.Simulate(0.01f);
        Physics.autoSimulation = true;
        NavMesh.RemoveAllNavMeshData();

        whileCheck = true;
        List<GameObject> currentListOptions = new List<GameObject>();
        for (int i = 0; i < masterList.Count; i++)
        {
            currentListOptions.Add(masterList[i]);
        }
        currentLevelDifficulty = 0;

        ShuffleRoomList(currentListOptions);
        Instantiate(dropShipRoom);
        currentExitLocation = dropShipRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
        int randomLayout = Random.Range(0, dropShipRoom.GetComponent<Room>().Layouts.Count);
        dropShipRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
        NavMeshSurface[] navComponents = dropShipRoom.GetComponentsInChildren<NavMeshSurface>();
        foreach (NavMeshSurface comp in navComponents)
        {
            comp.BuildNavMesh();
        }
        playerObject.transform.position = dropShipRoom.GetComponent<Room>().Entrance.position;

        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;
        currentListOptions = getBiomeSpecificList(currentListOptions, currentLevel);   //makes list biome specific                                                                                       //scale level difficulty
        desiredLevelDifficulty = ScaleDifficulty();
        while (currentLevelDifficulty < desiredLevelDifficulty && whileCheck == true)
        {
            genTest = InstantiateValidRoom(currentListOptions);
            if (genTest == false)
            {
                Debug.Log("FALSE LEVEL RETURNED (room)");
                genTest = false;
                yield break;
            }
        }
        if (genTest == true)
        {
            genTest = InstantiateEndRoom(endRoom);
        }
        if (genTest == true)
        {
            Debug.Log("TRUUUE LEVEL RETURNED (room)");
        }
        yield return null;
    }

    private void DestroyInstantiatedRooms()
    {
        GameObject[] iEntrances = GameObject.FindGameObjectsWithTag("RoomEntrance");
        for (int i = 0; i < iEntrances.Length; i++)
        {
            Destroy(iEntrances[i]);
        }
        GameObject[] iRooms = GameObject.FindGameObjectsWithTag("InstantiatedRoom");
        for (int i = 0; i < iRooms.Length; i++)
        {
            Destroy(iRooms[i]); 
        }
    }
    private void randomizeBiomes()
    {
        for (int i = 0; i < 6; i++)
        {
            int fill = Random.Range(0, 2);
            if (fill == 0)
            {
                levelBiomesList.Add(DroseraGlobalEnums.Biome.Jungle);
            }
            else if (fill == 1)
            {
                levelBiomesList.Add(DroseraGlobalEnums.Biome.Desert);
            }
        }
    }
    private List<GameObject> getBiomeSpecificList(List<GameObject> overallList, int currentLevel)
    {
        List<GameObject> biomeSpecificList = new List<GameObject>();
        for (int i = 0; i < overallList.Count; i++)
        {
            if (overallList[i].GetComponent<Room>().Biome == levelBiomesList[currentLevel - 1] ||
                overallList[i].GetComponent<Room>().Biome == DroseraGlobalEnums.Biome.None)
                biomeSpecificList.Add(overallList[i]);
        }
        return biomeSpecificList;
    }
}
