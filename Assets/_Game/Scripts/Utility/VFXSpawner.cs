using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpawner : MonoBehaviour {

    public static VFXSpawner vfx;

    private void Awake() {
        if(vfx == null) {
            vfx = this;
        } else {
            Destroy(gameObject);
        }
    }

    public GameObject SpawnVFX(GameObject effect, float lifetime, Vector3 position) {
        return SpawnVFX(effect, lifetime, position, Quaternion.identity);
    }

    public GameObject SpawnVFX(GameObject effect, float lifetime, Vector3 position, Quaternion rotation) {
        GameObject fx = Instantiate(effect, position, rotation);
        StartCoroutine(KillVFX(fx, lifetime));
        return fx;
    }

    private IEnumerator KillVFX(GameObject effect, float lifetime) {
        for(float i = 0; i < lifetime; i += Time.deltaTime)
            yield return null;
        Destroy(effect);
    }

}