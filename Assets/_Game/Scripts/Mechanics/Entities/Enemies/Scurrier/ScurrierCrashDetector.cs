using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScurrierCrashDetector : MonoBehaviour {

    public bool crash;

    private void OnEnable() {
        crash = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(((1 << other.gameObject.layer) & LayerMask.GetMask("Terrain", "BreakableWall")) != 0) {
            crash = true;
            gameObject.SetActive(false);
        }
    }

}