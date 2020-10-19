using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenTest : MonoBehaviour
{
    /// <summary>
    /// DEAD SCRIPT DO NOT USE
    /// </summary>
    [SerializeField]
    private int levelNumber = 1;
    public int LevelNumber { get => levelNumber; }

    [Header("Master Room Prefab")]
    [SerializeField]
    private GameObject roomMasterPrefab;
    public GameObject RoomMasterPrefab { get => roomMasterPrefab; }
    private List<GameObject> roomMasterList = new List<GameObject>();
    [Header("Level Biomes")]
    [SerializeField]
    private DroseraGlobalEnums.Biome level1Biome;
    public DroseraGlobalEnums.Biome Level1Biome { get => level1Biome; set => level1Biome = value; }
    [SerializeField]
    private DroseraGlobalEnums.Biome level2Biome;
    public DroseraGlobalEnums.Biome Level2Biome { get => level2Biome; set => level2Biome = value; }
    [SerializeField]
    private DroseraGlobalEnums.Biome level3Biome;
    public DroseraGlobalEnums.Biome Level3Biome { get => level3Biome; set => level3Biome = value; }
    [SerializeField]
    private DroseraGlobalEnums.Biome level4Biome;
    public DroseraGlobalEnums.Biome Level4Biome { get => level4Biome; set => level4Biome = value; }
    [SerializeField]
    private DroseraGlobalEnums.Biome level5Biome;
    public DroseraGlobalEnums.Biome Level5Biome { get => level5Biome; set => level5Biome = value; }
    [SerializeField]
    private DroseraGlobalEnums.Biome level6Biome;
    public DroseraGlobalEnums.Biome Level6Biome { get => level6Biome; set => level6Biome = value; }
    [SerializeField]
    private List<DroseraGlobalEnums.Biome> levelBiomesList = new List<DroseraGlobalEnums.Biome>();
    public List<DroseraGlobalEnums.Biome> LevelBiomesList { get => levelBiomesList; }

    [Header("Level Information")]
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

    [Header("Debugging")]
    public List<GameObject> showList = new List<GameObject>();

    //Variables to detect Entrance/Exit Rotations
    private Vector3 currentExitLocation;
    private float priorRoomExitRotation = 0;    //default should = dropship room exit rotation
    private float currentLevelRotation = 0;     //to prevent room spiraling with angular arrangements
    private Quaternion priorRoomRotation;
    private bool whileCheck = true;

    void Start() //on scene start, generate level
    {
        putBiomesInList();
        for (int i = 0; i < roomMasterPrefab.GetComponent<StoreRooms>().AllRooms.Count; i++)    //puts Prefab list into editable copy
        {
            roomMasterList.Add(roomMasterPrefab.GetComponent<StoreRooms>().AllRooms[i]);
        }
        List<GameObject> roomPrefabs = new List<GameObject>();      //list for first room
        for (int i = 0; i< roomMasterList.Count; i++)
        {
            roomPrefabs.Add(roomMasterList[i]);
        }
        ShuffleRoomList(roomPrefabs);       //randomizes room array
        currentExitLocation = dropShipRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);    //sets starting point and dropship exit
        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;    //get rotation of dropship exit

        Debug.Log("Generate Level: " + levelNumber);
        while (currentLevelDifficulty < desiredLevelDifficulty && whileCheck == true && roomPrefabs.Count>0)       //instatiate rooms till desiredDiff reached
        {
            InstantiateValidRoom(roomPrefabs, 1);   //first level generation
        }
        //end room
        InstantiateEndRoom(endRoom);
        Debug.Log("Finish Level: " + levelNumber);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            /*
            Debug.Log("Generate Level: " + (levelNumber+1));
            levelNumber += 1;
            NextLevel(roomMasterList, levelNumber);
            Debug.Log("Finish Level: " + levelNumber);
            */
            NextLevel(roomMasterList, 1);
            Debug.Log("Finish Level: " + levelNumber);
        }
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
    /// Checks needed: biome, rotation
    /// </summary>
    private void InstantiateValidRoom(List<GameObject> roomList, int currentLevel)
    {
        GameObject toRemove = null;
        for (int i = 0; i < roomList.Count; i++) //goes through list until a break (will break on chosen room/continues on failed room check)
        {           //check for room biome
            /*if (roomList[i].GetComponent<Room>().Biome == levelBiomesList[currentLevel - 1] || roomList[i].GetComponent<Room>().Biome == DroseraGlobalEnums.Biome.None)
            {        //will be room rotation check
                
                toRemove = roomList[i];
                GameObject plz = Instantiate(roomList[i], currentExitLocation + FixTransformDeficit(roomList[i]), priorRoomRotation);
                    plz.transform.RotateAround(currentExitLocation, Vector3.up, RotateRoom(priorRoomRotation, roomList[i]));
                //priorRoomRotation = plz.transform.rotation;
                priorRoomRotation = plz.GetComponent<Room>().Exit.transform.rotation;
                plz.GetComponent<Room>().Entrance.transform.SetParent(null);
                plz.transform.SetParent(plz.GetComponent<Room>().Entrance,true);
                plz.GetComponent<Room>().Entrance.transform.position = currentExitLocation;
                currentExitLocation = plz.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);

                //activate layout and add difficulty (get number of avaliable layouts)  Layouts.Count  Random.Range();
                int randomLayout = Random.Range(0, plz.GetComponent<Room>().Layouts.Count);
                plz.GetComponent<Room>().SetLayoutActive(randomLayout, true);
                        //Debug.Log("Layout Activated: " + plz.GetComponent<Room>().Layouts[randomLayout].name + " Diff: " + plz.GetComponent<Room>().Layouts[randomLayout].difficulty);
                currentLevelDifficulty += plz.GetComponent<Room>().Layouts[randomLayout].difficulty;
                break;
            }
            else if (i == (roomList.Count - 1))
            {
                break;  //breaks loop if last room is incompatible
            }*/
        }
        if (toRemove)
        {
        roomList.Remove(toRemove);

        }

        if (roomList.Count == 0)
        {
            whileCheck = false; //safety check in case room runs out defor desired difficulty
        }
    }

    private void InstantiateEndRoom(GameObject lastRoom)
    {
                GameObject plz = Instantiate(lastRoom, currentExitLocation + FixTransformDeficit(lastRoom), priorRoomRotation);
                plz.transform.RotateAround(currentExitLocation, Vector3.up, RotateRoom(priorRoomRotation, lastRoom));
                priorRoomRotation = plz.GetComponent<Room>().Exit.transform.rotation;
                plz.GetComponent<Room>().Entrance.transform.SetParent(null);
                plz.transform.SetParent(plz.GetComponent<Room>().Entrance, true);
                plz.GetComponent<Room>().Entrance.transform.position = currentExitLocation;
    }

    private Vector3 FixTransformDeficit(GameObject genericRoom)
    {
        return (genericRoom.transform.position - genericRoom.GetComponent<Room>().Entrance.transform.position);
    }

    private float RotateRoom(Quaternion previousRoom,GameObject genericRoom)
    {
        Quaternion adjustmentRotation = new Quaternion(0,0,0,0);
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

    void NextLevel(List<GameObject> masterList, int currentLevel) //on new level; similar to start function but additional stuff to erase.
    {
        Debug.Log("Next level function call: " + currentLevel);
        DestroyInstantiatedRooms();
        whileCheck = true;
        List<GameObject> currentListOptions = new List<GameObject>();
        for (int i = 0; i < masterList.Count; i++)
        {
            currentListOptions.Add(masterList[i]);
        }
        currentLevelDifficulty = 0;
        showList = masterList;
        ShuffleRoomList(currentListOptions);
        currentExitLocation = dropShipRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
                //add so can chose random layout for drop ship room.
        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;

        while (currentLevelDifficulty < desiredLevelDifficulty && whileCheck == true && currentListOptions.Count > 0)
        {
            InstantiateValidRoom(currentListOptions,currentLevel);
        }
        InstantiateEndRoom(endRoom);
    }

    private void DestroyInstantiatedRooms()
    {
        GameObject[] iEntrances = GameObject.FindGameObjectsWithTag("RoomEntrance"); //destroys entrances since they're new parents
        for (int i = 0; i < iEntrances.Length; i++)
        {
            Destroy(iEntrances[i]);
        }
    }

    private void putBiomesInList()
    {
        levelBiomesList.Add(level1Biome);
        levelBiomesList.Add(level2Biome);
        levelBiomesList.Add(level3Biome);
        levelBiomesList.Add(level4Biome);
        levelBiomesList.Add(level5Biome);
        levelBiomesList.Add(level6Biome);
    }

}
