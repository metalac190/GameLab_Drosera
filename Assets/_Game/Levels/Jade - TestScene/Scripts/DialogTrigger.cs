using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] public GameObject dialogTrigger;

    private void OnTriggerEnter(Collider other)
    {
        dialogTrigger.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        dialogTrigger.SetActive(false);
    }
}
