using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceScreen : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject player;
    [SerializeField] Vector3 baseOffset;
    [SerializeField] bool trackPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = Camera.main;
        if (player == null)
            player = FindObjectOfType<Gunner>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }

    private void LateUpdate()
    {
        if (trackPlayer)
        {
            Vector3 distanceFromPlayer = player.transform.position - transform.position;
            transform.position += baseOffset + distanceFromPlayer;
        }
    }
}
