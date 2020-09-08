using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenTest : MonoBehaviour
{
    /// <summary>
    /// A test script of level generation
    /// </summary>

    [Header("Room List for Level")]
    [Tooltip("List To plave available rooms for Level")]
    [SerializeField]
    private List<GameObject> roomPrefabs = new List<GameObject>();
    public List<GameObject> RoomPrefabs { get => roomPrefabs; }

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

    //Variables to detect Entrance/Exit Rotations
    public Vector3 currentExitLocation;
    private float priorRoomExitRotation = 0;    //default should = dropship room exit rotation
    private float currentLevelRotation = 0;     //to prevent room spiraling with angular arrangements
    private Quaternion priorRoomRotation;

    void Start() //on scene start, generate level
    {
        ShuffleRoomList(roomPrefabs);       //randomizes room array
        currentExitLocation = dropShipRoom.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
        priorRoomRotation = dropShipRoom.GetComponent<Room>().Exit.rotation;
        /*
        while(currentLevelDifficulty < desiredLevelDifficulty)
        {
            //call function
        }
        */
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            InstantiateValidRoom(roomPrefabs);
        }
    }


    /// <summary>
    /// This will randomize the roomList on start (shuffle the deck)
    /// </summary>
    private void ShuffleRoomList(List<GameObject> roomList)
    {
        Debug.Log("Random List: ");
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
    /// Checks:
    ///     Rotation
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
                //plz.transform.RotateAround(currentExitLocation,Vector3.up, RotateRoom(priorRoomRotation, roomList[i]));
                priorRoomRotation = plz.transform.rotation; //change to of exit
                //plz.transform.position = currentExitLocation + FixTransformDeficit(roomList[i]);
                currentExitLocation = plz.GetComponent<Room>().Exit.transform.TransformPoint(Vector3.zero);
                break;
            }
            //check room rotation
                //good --> go on; bad--> continue
        }
        //Debug.Log("Removed Object: " + toRemove);
        roomList.Remove(toRemove);
    }

    private Vector3 FixTransformDeficit(GameObject genericRoom)
    {
        return (genericRoom.transform.position - genericRoom.GetComponent<Room>().Entrance.transform.position);
    }


    private float RotateRoom(Quaternion previousRoom,GameObject genericRoom)
    {
        Quaternion adjustmentRotation = new Quaternion(0,0,0,0);
        //Debug.Log("Previous_rot: " + previousRoom.eulerAngles + " NewRoom_rot " + genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles);
        //float angleDifferenceFloat = Quaternion.Angle(previousRoom, genericRoom.GetComponent<Room>().Entrance.transform.rotation);
        float neededRotate = 0; //the y rotation needed to match
        //Debug.Log("Angle difference: " + angleDifferenceFloat);


        if (genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y > 180)
        {
            neededRotate = (360 - genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y);
            Debug.Log("Correction: " + neededRotate);
        }
        else if (genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y <= 180)
        {
            neededRotate = -(genericRoom.GetComponent<Room>().Entrance.transform.rotation.eulerAngles.y);
            Debug.Log("Correction: " + neededRotate);
        }
        //Debug.Log("New Rotation: " + (adjustmentRotation.eulerAngles = new Vector3(0, neededRotate, 0)));
        //Debug.Log("Would be new rotation");
        return neededRotate;
    }
}
