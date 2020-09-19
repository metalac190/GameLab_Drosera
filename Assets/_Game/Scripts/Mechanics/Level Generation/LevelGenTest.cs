using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenTest : MonoBehaviour
{
    /// <summary>
    /// A test script of level generation
    /// </summary>
    /// 

    [SerializeField]
    private int levelNumber = 1;
    public int LevelNumber { get => levelNumber; }

    [Header("Room List for Level")]
    [Tooltip("List To plave available rooms for Level")]
    [SerializeField]
    private List<GameObject> roomPrefabsLevel1Master = new List<GameObject>();
    public List<GameObject> RoomPrefabsLevel1Master { get => roomPrefabsLevel1Master; }

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

    //Variables to detect Entrance/Exit Rotations
    private Vector3 currentExitLocation;
    private float priorRoomExitRotation = 0;    //default should = dropship room exit rotation
    private float currentLevelRotation = 0;     //to prevent room spiraling with angular arrangements
    private Quaternion priorRoomRotation;
    private bool whileCheck = true;

    void Start() //on scene start, generate level
    {
        List<GameObject> roomPrefabs = new List<GameObject>();
        for (int i = 0; i< roomPrefabsLevel1Master.Count; i++)
        {
            roomPrefabs.Add(roomPrefabsLevel1Master[i]);
        }

        ShuffleRoomList(roomPrefabs);       //randomizes room array
        currentExitLocation = dropShipRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;

        while (currentLevelDifficulty < desiredLevelDifficulty && whileCheck == true)       //instatiate rooms till desiredDiff reached
        {
            InstantiateValidRoom(roomPrefabs);
        }
        //end room
        InstantiateEndRoom(endRoom);
    }
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //InstantiateValidRoom(roomPrefabs); 
            //InstantiateEndRoom(endRoom);                //***********//
            NextLevel(roomPrefabsLevel1Master);
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
    /// </summary>
    private void InstantiateValidRoom(List<GameObject> roomList)
    {
        GameObject toRemove = null;
        float blank = 0;
        for (int i = 0; i < roomList.Count; i++) //goes through list until a break (will break on chosen room/continues on failed room check)
        {
            if (blank == 0){        //will be room rotation check
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
        }
        roomList.Remove(toRemove);

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
                //currentExitLocation = plz.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);

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

    void NextLevel(List<GameObject> masterList) //on new level; similar to start function but additional stuff to erase.
    {
        levelNumber += 1;
        DestroyInstantiatedRooms();
        whileCheck = true;
        List<GameObject> currentListOptions = new List<GameObject>();
        for (int i = 0; i < masterList.Count; i++)
        {
            currentListOptions.Add(masterList[i]);
        }
        currentLevelDifficulty = 0;

        ShuffleRoomList(currentListOptions);
        currentExitLocation = dropShipRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
                //add so can chose random layout for drop ship room.
        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;

        while (currentLevelDifficulty < desiredLevelDifficulty && whileCheck == true)
        {
            InstantiateValidRoom(currentListOptions);
        }
        InstantiateEndRoom(endRoom);
    }

    private void DestroyInstantiatedRooms()
    {
        GameObject[] iRooms = GameObject.FindGameObjectsWithTag("InstantiatedRoom");
        for (int i = 0; i < iRooms.Length; i++)
        {
            Destroy(iRooms[i]);
        }
        GameObject[] iEntrances = GameObject.FindGameObjectsWithTag("RoomEntrance");
        for (int i = 0; i < iEntrances.Length; i++)
        {
            Destroy(iEntrances[i]);
        }
    }

}
