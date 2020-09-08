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
    [Tooltip("The difficulty at end of level. DO NOT EDIT")]
    [SerializeField]
    private float currentLevelDifficulty = 0; 
    public float CurrentLevelDifficulty { get => currentLevelDifficulty; set => currentLevelDifficulty = value; }

    //Variables to detect Entrance/Exit Rotations
    private float priorRoomExitRotation = 0;    //default should = dropship room exit rotation
    private float currentLevelRotation = 0;     //to prevent room spiraling with angular arrangements


    void Start() //on scene start, generate level
    {
        ShuffleRoomList(roomPrefabs);       //randomizes room array

        /*
        while(currentLevelDifficulty < desiredLevelDifficulty)
        {
            //call function
            currentLevelDifficulty += 1; //placeholder so while loop breaks
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

    //shuffle a list
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
    private void InstantiateValidRoom(List<GameObject> roomList)
    {
        GameObject toRemove = null;
        for (int i = 0; i < roomList.Count; i++) //goes through list until a break
        {
            toRemove = roomList[i];
            Instantiate(roomList[i]);
            break;

            //check room rotation
                //good --> go on; bad--> continue
        }
        Debug.Log("Removed Object: " + toRemove);
        roomList.Remove(toRemove);
        Debug.Log("Removed.");


    }


}
