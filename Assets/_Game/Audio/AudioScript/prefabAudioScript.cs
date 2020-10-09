/*
THE AudioScript.cs BELONGS ON THE SAME OBJECT AS THE SCRIPT THAT IS TRIGGERING THE SOUND.
THE prefabAudioScript.cs BELONGS ON AN EMPTY GAMEOBJECT PREFAB ALONG WITH AN AUDIO SOURCE (like the example "SoundPrefab" game object, which you can use).
***
An explaination of AudioScript.cs is in the AudioScript.cs code. This script doesn't do anything on it's own, and its functions are called by AudioScript.cs.
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prefabAudioScript : MonoBehaviour
{
    
    AudioSource audioSource;
    [Header("Go to the AudioScript to edit the sound's properties.")]
    [Tooltip("No,this bool doesn't do anything.")]
    public bool GotIt;
    [HideInInspector]
    public AudioClip[] audioClips;
    [HideInInspector]
    public bool persistentAudio = false;
    [HideInInspector]
    public bool loop = false;
    [HideInInspector]
    public bool randomize = false;
    [HideInInspector]
    public bool smartRandomize = false;
    [HideInInspector]
    public bool stopSoundOnFadeOut = false;
    [HideInInspector]
    public bool persistentSoundOver = false;
    [HideInInspector]
    public float StartVolume = 1.0f;
    [HideInInspector]
    public float FadeInSpeed = 0.0f;
    [HideInInspector]
    public float FadeInEndVolume = 1.0f;
    [HideInInspector]
    public float FadeOutSpeed = 0.0f;
    [HideInInspector]
    public float FadeOutEndVolume = 0.0f;
    [HideInInspector]
    public float RandomPitchMax = 1.0f;
    [HideInInspector]
    public float RandomPitchMin = 1.0f;
    [HideInInspector]
    public float RandomVolumeMax = 1.0f;
    [HideInInspector]
    public float RandomVolumeMin = 1.0f;
    [HideInInspector]
    public float persistentSoundDelay = 0.0f;
    int ranClip;
    int lastRanClip;
    private float ranVol;
    private float ranPit;
    private bool isFadingOut = false;
    private bool isFadingIn = false;
    private bool firstPlay = true;
    private IEnumerator fadeSoundIn;
    

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastRanClip = Random.Range(0, audioClips.Length);
    }

    public void FadeSoundIn()
    {
        audioSource = GetComponent<AudioSource>();
        isFadingIn = true;
        fadeSoundIn = FadeAudioIn(audioSource, FadeInSpeed, FadeInEndVolume);
        StartCoroutine(fadeSoundIn);
    }

    public void FadeSoundOut()
    {
        audioSource = GetComponent<AudioSource>();
        isFadingOut = true;
        IEnumerator fadeSoundOut = FadeAudioOut(audioSource, FadeOutSpeed, FadeOutEndVolume);
        StartCoroutine(fadeSoundOut);
    }

    public IEnumerator FadeAudioIn(AudioSource audioSource, float FadeInSpeed, float FadeInEndVolume)
    {
        if (audioSource.volume <= FadeInEndVolume && audioSource != null)
        {
            while (audioSource.volume < FadeInEndVolume)
            {
                audioSource.volume += Time.deltaTime * (FadeInSpeed * 0.1f);
                yield return null;
            }
        }
        else
        {
            Debug.Log("Your audio is not fading in because the starting volume is a higher value than your FadeInStartVolume");
            yield return null;
        }
    }

    public IEnumerator FadeAudioOut(AudioSource audioSource, float FadeOutSpeed, float FadeOutEndVolume)
    {
        if (audioSource.volume >= FadeOutEndVolume && audioSource != null)
        {
            while (audioSource.volume > FadeOutEndVolume)
            {
                audioSource.volume -= Time.deltaTime * FadeOutSpeed * 0.1f;
                yield return null;
            }
        } else
        {
            Debug.Log("Your audio is not fading out because the starting volume is a lower value than your FadeOutStartVolume");
            yield return null;
        }
    }

    void DestroySoundOneShot()
    {
        if (audioSource.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (persistentAudio == true)
        {
            if (audioSource.isPlaying == false)
            {
                if (persistentSoundOver == false)
                {
                    if (audioClips.Length > 1)
                    {
                        if (smartRandomize)
                        {
                            ranClip = Random.Range(0, audioClips.Length);
                            while (ranClip == lastRanClip)
                            {
                                ranClip = Random.Range(0, audioClips.Length);
                            }
                            audioSource.clip = audioClips[ranClip];
                        }
                        else if (randomize)
                        {
                            ranClip = Random.Range(0, audioClips.Length);
                            audioSource.clip = audioClips[ranClip];
                        }
                        else
                        {
                            audioSource.clip = audioClips[0];
                        }
                    }
                    else
                    {
                        audioSource.clip = audioClips[0];
                    }

                    if (RandomPitchMax > RandomPitchMin)
                    {
                        ranPit = Random.Range(RandomPitchMin, RandomPitchMax);
                        audioSource.pitch = ranPit;
                    }
                    else
                    {
                        audioSource.pitch = 1.0f;
                    }

                    if (RandomVolumeMax > RandomVolumeMin) 
                    {
                        if (isFadingIn != true && isFadingOut != true) {
                            ranVol = Random.Range(RandomVolumeMin, RandomVolumeMax);
                            audioSource.volume = ranVol;
                        }
                    }
                    else
                    {
                        if (isFadingIn != true && isFadingOut != true)
                        {
                            audioSource.volume = StartVolume;
                        }
                    }

                    if (persistentSoundDelay > 0 && firstPlay == false)
                    {
                        audioSource.PlayDelayed(persistentSoundDelay);
                    } else {
                        audioSource.Play();
                        firstPlay = false;
                    }

                    if (smartRandomize)
                    {
                        lastRanClip = ranClip;
                    }
                }
                else
                {
                    DestroySoundOneShot();
                }
            }
        }
        else
        {
            DestroySoundOneShot();
        }
        if (isFadingOut && stopSoundOnFadeOut)
        {
            persistentAudio = false;
            if (loop && audioSource.isPlaying)
            {
                loop = false;
                Destroy(gameObject, audioSource.clip.length*0+(20/FadeOutSpeed));
            }
        }
        if (isFadingOut && isFadingIn)
        {
            StopCoroutine(fadeSoundIn);
        }
    }
}