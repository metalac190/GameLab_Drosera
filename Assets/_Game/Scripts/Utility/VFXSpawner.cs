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

    public void SpawnVFX(GameObject effect, float lifetime, Vector3 position) {
        SpawnVFX(effect, lifetime, position, Quaternion.identity);
    }

    public void SpawnVFX(GameObject effect, float lifetime, Vector3 position, Quaternion rotation) {
        GameObject fx = Instantiate(effect, position, rotation);
        StartCoroutine(KillVFX(fx, lifetime));
    }

    private IEnumerator KillVFX(GameObject effect, float lifetime) {
        for(float i = 0; i < lifetime; i += Time.deltaTime)
            yield return null;
        Destroy(effect);
    }

}