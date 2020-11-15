using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticlesWhilePaused : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale < 0.01f)
        {
            particle.Simulate(Time.unscaledDeltaTime, true, false);
        }
        else if (particle.isPaused)
        {
            particle.Play();
        }
    }
}
