using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Debug.Log("Shake called");
        Vector3 originalPos = transform.localPosition;
        float elasped = 0.0f;
        while (elasped < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elasped += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }

    public void TriggerShakeLight(float duration)
    {
        Shake(duration, .1f);
    }

    public void TriggerShakeMedium(float duration)
    {
        Shake(duration, .5f);
    }

    public void TriggerShakeHeavy(float duration)
    {
        Shake(duration, 1f);
    }
}
