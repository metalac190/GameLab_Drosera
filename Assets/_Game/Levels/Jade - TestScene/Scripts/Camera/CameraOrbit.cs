using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] float rotateSpeed = 30.0f;

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.X))
        {
            transform.RotateAround(cameraTarget.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            transform.RotateAround(cameraTarget.position, -Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
