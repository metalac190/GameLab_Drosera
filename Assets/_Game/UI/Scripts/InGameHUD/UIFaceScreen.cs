using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceScreen : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject player;
    [SerializeField] Vector3 baseOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }

    private void LateUpdate()
    {
        Vector3 distanceFromPlayer = player.transform.position - transform.position;
        transform.position += baseOffset + distanceFromPlayer;
    }
}
