using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemTest : MonoBehaviour
{
    [Tooltip("The button to press to play the particle system")]
    [SerializeField] KeyCode testKey = KeyCode.E;
    [SerializeField] bool inTestMode = true;
    ParticleSystem[] theParticles;

    private void Awake()
    {
        theParticles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inTestMode && Input.GetKeyDown(testKey))
        {
            PlayParticles();
        }
    }

    public void PlayParticles()
    {
        foreach (ParticleSystem particle in theParticles)
        {
            particle.Play();
        }
    }
}
