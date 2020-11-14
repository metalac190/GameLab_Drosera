using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool testShake = false;
    public float testDuration = 1f;
    public float testMagnitude = 1f;

    public IEnumerator Shake(float duration, float magnitude)
    {
        Debug.Log("Shake called");
       Vector3 originalPos = transform.localPosition;

        float elasped = 0.0f;
        while (elasped < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, + originalPos.z);
            elasped += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void TriggerCameraShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public void TriggerShakeLight(float duration)
    {
        StartCoroutine(Shake(duration, .1f));
    }

    public void TriggerShakeMedium(float duration)
    {
        StartCoroutine(Shake(duration, .5f));
    }

    public void TriggerShakeHeavy(float duration)
    {
        StartCoroutine(Shake(duration, 1f));
    }

    private void Update()
    {
        if(testShake)
        {
            testShake = false;
            StartCoroutine(Shake(testDuration, testMagnitude));
        }
    }
}
