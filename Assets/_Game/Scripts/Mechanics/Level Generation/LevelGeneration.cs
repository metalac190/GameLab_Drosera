using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    /// <summary>
    /// Things to be added after alpha
    ///     re-add biome randomization --> uncomment randomize biome and the specification function
    ///     
    /// </summary>
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
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GenerateLevelTrigger();
        }
    }
    
    public void GenerateLevelTrigger()
    {
        StartCoroutine(GenerateLevelCoroutine());
    }
    IEnumerator GenerateLevelCoroutine()
    {
        Debug.Log("Level Number: " + LevelNumber);
        genTest = false;
        if (levelNumber < 6)
        {
            while (genTest == false)
            {
                StartCoroutine(CreateLevelCoroutine(levelNumber, roomMasterPrefab.GetComponent<StoreRooms>().AllRooms));
                yield return new WaitForSeconds(.001f);
            }
            levelNumber += 1;
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
                GameObject testRoom = Instantiate(roomList[i], currentExitLocation + FixTransformDeficit(roomList[i]), priorRoomRotation);
                testRoom.transform.RotateAround(currentExitLocation, Vector3.up, RotateRoom(priorRoomRotation, roomList[i]));
                //priorRoomRotation = plz.transform.rotation;
                priorRoomRotation = testRoom.GetComponent<Room>().Exit.transform.rotation;
                testRoom.GetComponent<Room>().Entrance.transform.SetParent(null);
                testRoom.transform.SetParent(testRoom.GetComponent<Room>().Entrance, true);
                testRoom.GetComponent<Room>().Entrance.transform.position = currentExitLocation;
                currentExitLocation = testRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
                Physics.autoSimulation = false;
                Physics.Simulate(0.001f);
                Physics.autoSimulation = true;

                if (testRoom.GetComponent<Room>().overlapping == true)
                {
                    roomCheck = false;
                }
                testRoom.GetComponent<Room>().SetBiomeActive(levelBiomesList[levelNumber], true);

                //activate layout and add difficulty (get number of avaliable layouts)  Layouts.Count  Random.Range();
                if (testRoom.GetComponent<Room>().data != null)
                {
                    int randomLayout = Random.Range(0, testRoom.GetComponent<Room>().data.Layouts.Count);
                    testRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
                    currentLevelDifficulty += testRoom.GetComponent<Room>().data.Layouts[randomLayout].difficulty;
                }
                else
                {
                    int randomLayout = Random.Range(0, testRoom.GetComponent<Room>().Layouts.Count);
                    testRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
                    currentLevelDifficulty += testRoom.GetComponent<Room>().Layouts[randomLayout].difficulty;
                }
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
        Debug.Log("Level Biome: " + LevelBiomesList[LevelNumber]);
        bool roomCheck = true;
        GameObject hyperseedRoom = Instantiate(lastRoom, currentExitLocation + FixTransformDeficit(lastRoom), priorRoomRotation);
        hyperseedRoom.transform.RotateAround(currentExitLocation, Vector3.up, RotateRoom(priorRoomRotation, lastRoom));
        priorRoomRotation = hyperseedRoom.GetComponent<Room>().Exit.transform.rotation;
        hyperseedRoom.GetComponent<Room>().Entrance.transform.SetParent(null);
        hyperseedRoom.transform.SetParent(hyperseedRoom.GetComponent<Room>().Entrance, true);
        hyperseedRoom.GetComponent<Room>().Entrance.transform.position = currentExitLocation;
        hyperseedRoom.GetComponent<Room>().SetBiomeActive(levelBiomesList[levelNumber], true);
        if(hyperseedRoom.GetComponent<Room>().data != null)
        {
            int randomLayout = Random.Range(0, hyperseedRoom.GetComponent<Room>().data.Layouts.Count);
            hyperseedRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
            Debug.Log("EndRoom: " + hyperseedRoom.GetComponent<Room>().data.Layouts[randomLayout].name);
        }
        else
        {
            int randomLayout = Random.Range(0, hyperseedRoom.GetComponent<Room>().Layouts.Count);
            hyperseedRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
            Debug.Log("EndRoom: " + hyperseedRoom.GetComponent<Room>().Layouts[randomLayout].name);
        }
        
        
        Physics.autoSimulation = false;
        Physics.Simulate(0.001f);
        Physics.autoSimulation = true;
        
        if (hyperseedRoom.GetComponent<Room>().overlapping == true)
        {
            roomCheck = false;
        }
        hyperseedRoom.GetComponentInChildren<NavMeshGenerator>().BuildNavMesh();
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
        yield return null;
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
        dropShipRoom.GetComponent<Room>().SetBiomeActive(levelBiomesList[levelNumber], true);
        playerObject.transform.position = dropShipRoom.GetComponent<Room>().Entrance.position;
        if(dropShipRoom.GetComponent<Room>().data != null)
        {
            int randomLayout = Random.Range(0, dropShipRoom.GetComponent<Room>().data.Layouts.Count);
            dropShipRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
            Debug.Log("Dropship: " + dropShipRoom.GetComponent<Room>().data.Layouts[randomLayout].name);
        }
        else
        {
            int randomLayout = Random.Range(0, dropShipRoom.GetComponent<Room>().Layouts.Count);
            dropShipRoom.GetComponent<Room>().SetLayoutActive(randomLayout, true);
            Debug.Log("Dropship: " + dropShipRoom.GetComponent<Room>().Layouts[randomLayout].name);
        }
        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;
        desiredLevelDifficulty = ScaleDifficulty();
        while (currentLevelDifficulty < desiredLevelDifficulty && whileCheck == true)
        {
            genTest = InstantiateValidRoom(currentListOptions);
            if (genTest == false)
            {
                genTest = false;
                yield break;
            }
        }
        if (genTest == true)
        {
            genTest = InstantiateEndRoom(endRoom);
        }
        if (genTest == false)   //last check to see if hyperseed overlaps
        {
            genTest = false;
            yield break;
        }
        ToonHelper.InitializeAllLighting();
        yield return null;
    }

    private void DestroyInstantiatedRooms()
    {
        GameObject[] iEntrances = GameObject.FindGameObjectsWithTag("RoomEntrance");
        for (int i = 0; i < iEntrances.Length; i++)
        {
            //iEntrances[i].SetActive(false);
            Destroy(iEntrances[i]);
        }
        GameObject[] iRooms = GameObject.FindGameObjectsWithTag("InstantiatedRoom");
        for (int i = 0; i < iRooms.Length; i++)
        {
            //iRooms[i].SetActive(false);
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
}
