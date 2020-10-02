using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    private float rotateSpeed = 30.0f;

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(cameraTarget.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(cameraTarget.position, -Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
