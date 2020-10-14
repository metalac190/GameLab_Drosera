using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] float rotateSpeed = 30.0f;

    PlayerBase player;

    private void Start()
    {
        player = PlayerBase.instance;
    }

    void LateUpdate()
    {
        if (player.AdjustCameraRight)
        {
            transform.RotateAround(cameraTarget.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }

        if (player.AdjustCameraLeft)
        {
            transform.RotateAround(cameraTarget.position, -Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
