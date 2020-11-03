using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundSingleton : MonoBehaviour
{
    public static EnemySoundSingleton instance = null;
    private bool _damageTakenSoundActive = false;
    public bool DamageTakenSoundActive
    {
        get
        {
            return _damageTakenSoundActive;
        }

        set
        {
            _damageTakenSoundActive = value;
            if (value == true)
            {
                StartCoroutine(EnemyDamagedSoundDelay(0.5f));
            }
        }
    }


    void Awake() {
        instance = this;
    }

    public IEnumerator EnemyDamagedSoundDelay(float delayTime)
    {
        //Debug.Log("Damage Taken: true");
        _damageTakenSoundActive = true;  //activates a stopper to prevent sound overlap
        yield return new WaitForSeconds(delayTime);   //delay for next sound after a sounds activation
        _damageTakenSoundActive = false; //allows next enemy damage sound to be activated
        //Debug.Log("Damage Taken: false");
        yield return null;
    }
}
