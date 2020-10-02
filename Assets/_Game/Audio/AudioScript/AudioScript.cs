/*
 * Developed by: Joseph Nied
THE AudioScript.cs BELONGS ON THE SAME OBJECT AS THE SCRIPT THAT IS TRIGGERING THE SOUND.
THE persistentAudioScript.cs BELONGS ON AN EMPTY GAMEOBJECT PREFAB ALONG WITH AN AUDIO SOURCE (like the example "SoundPrefab" game object, which you can use).
***
This script allows you to quickly implement all kinds of audio.

To use it,

1) Make a prefab using an empty game object with an audio source component and the prefabAudioScript.cs code. YOU ONLY NEED ONE OF THESE PREFABS FOR THE WHOLE 
    PROJECT (Unless you want many different 3D audio settings for some sounds. If you want that, just make different prefabs the same way, 
   but with the different 3D sound setting in the AudioSource components of each prefab in the inspector. If you don't care about 3D audio settings,
   you'll most likely just need one prefab, and that prefab can be the given "SoundPrefab" game object).

2) Put the AudioScript.cs code on the gameobject with the script that you want to trigger a sound.

3) Use the following code within the other script to call the PlaySound function from this script:

    ***PLAYING SOUNDS***
    Put these lines directly in the code where you want the sound to trigger:

                AudioScript audioScript = GetComponent<AudioScript>();
                audioScript.PlaySound(0);

    (The 0 refers to which audio clip you want to trigger on the script. This should only be a number other than 0 if you want to only play a single sound
    and that sound isn't the first audio clip on the script in the inspector.)

    If you want to play different sounds simulataneously from the same gameobject, just add another AudioScript.cs component to the gameobject and use 
    this code to trigger sounds:

                AudioScript[] audioScript = GetComponents<AudioScript>();
                audioScript[0].PlaySound(0);
                            ^ This number is which AudioScript.cs component you want to play. If you had 2 AudioScript.cs components, for example, it might be a 1.

    ******************************
    ***FADING SOUNDS IN AND OUT***
    Put these lines directly in the code where you want the sound to fade in or out:

                AudioScript audioScript = GetComponent<AudioScript>();
                audioScript.FadeSoundIn();

    or 

                AudioScript audioScript = GetComponent<AudioScript>();
                audioScript.FadeSoundOut();

    Of course, if you're trying to fade in or out a sound from one of multiple scripts on the same game object, you have to specify which script like so:

                AudioScript[] audioScript = GetComponents<AudioScript>();
                audioScript[0].FadeSounIn(0);
                            ^ This number is which AudioScript.cs component you want to fade. If you had 2 AudioScript.cs components, for example, it might be a 1.
    
    Fading a sound in or out ONLY WORKS if a sound IS ALREADY PLAYING. Make a sound both play and fade in could look like this:

                AudioScript audioScript = GetComponent<AudioScript>();
                audioScript.PlaySound(0);
                audioScript.FadeSoundOut();

    If you want a sound to stop playing when it fades out, just use the FadeSoundOut() function above but check the boolean "Stop Sound On Fade Out" in the inspector.

    *****************************
    ***STOPPING SOUNDS***
    Put these lines directly in the code where you want the sound to fade in or out:

                AudioScript audioScript = GetComponent<AudioScript>();
                audioScript.StopSound(0);

    To stop a sound without a harsh cutoff, check the "Let Clip Finish On Stop" element in the inspector.

    If you want a sound to stop playing when it fades out, just use the FadeSoundOut() function and check the boolean "Stop Sound On Fade Out" in the inspector.
    *****************************

4) Set the inspector variables. You can hover over each item in the inspector for an explaination of each.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioScript : MonoBehaviour
{
    [Header("Necessary Settingss")]
    [Tooltip("The prefab with the audioSource and prefabAudioScript.cs script.")]
    public GameObject audioPrefab;
    [Tooltip("The parent of the audio prefab. Can be left blank if there is no parent. The parent can be the prefab this script is attached to, and often will be.")]
    public GameObject audioPrefabParent;
    GameObject audioPrefabClone;
    [Tooltip("The mixer track you want to send the audio to. THIS IS HIGHLY RECOMMENDED, though not necessary.")]
    public AudioMixerGroup mixerTrack;
    [Tooltip("The audio clips you want to play/include.")]
    public AudioClip[] audioClips;
    [Header("Volume and Pan")]
    [Space(15)]
    [Tooltip(" The volume the audio starts at. It should basically always be 1 (if the sound plays right away) or 0 (if the sound fades in). If you don't fade the sound in or out, this is just volume.")]
    [Range(0, 1)]
    public float StartVolume = 1.0f;
    [Tooltip("The stereo pan of the sound/sounds.")]
    [Range(-1, 1)]
    public float StereoPan = 0.0f;
    [Header("Looping and Playlists")]
    [Space(10)]
    [Tooltip("Make the sound loop.")]
    public bool loop;
    [Tooltip("Check this if you want to play from the clips you've added until the you call stop or fade out (with 'Stop Sound On Fade Out' checked) functions are called.")]
    public bool useClipsAsPlaylist;
    [Tooltip("Sets the delay between the audio clips when 'Use Clips As Playlist' is selected. Set to 0 if you don't want a delay.")]
    [Range(0, 10)]
    public float playlistClipDelay = 0.0f;
    [Tooltip("Randomizes the audio clips whenever the sound is triggered to play or each time a playlist clip plays.")]
    public bool randomize;
    [Tooltip("Randomizes sound but will never play the same sound twice in a row. 'Smart Randomize' overrides 'Randomize' if they're both checked.")]
    public bool smartRandomize;
    [Header("Stop Settings")]
    [Space(10)]
    [Tooltip("Stops the sound once it's faded out. Note that you can't fade the sound back in again without calling the PlaySound() function.")]
    public bool stopSoundOnFadeOut = false;
    [Tooltip("Lets the clip playing when the StopSound() function is called finish. Useful when using playlists.")]
    public bool letClipFinishOnStop = false;
    [Header("Fade Settings")]
    [Space(10)]
    [Tooltip("Sets the fade in speed. Lower numbers are slower speeds while higher numbers are faster.")]
    [Range(0.1f, 20)]
    public float FadeInSpeed=0.0f;
    [Tooltip("Sets the volume when the sound fades in.")]
    [Range(0, 1)]
    public float FadeInEndVolume = 1.0f;
    [Space(5)]
    [Tooltip("Sets the fade out speed. Lower numbers are slower speeds while higher numbers are faster.")]
    [Range(0.1f, 20)]
    public float FadeOutSpeed=0.0f;
    [Tooltip("Sets the volume when the sound fades out. Should be 0 if you want to the stop the sound after fade out.")]
    [Range(0, 1)]
    public float FadeOutEndVolume = 0.0f;
    [Header("Randomization")]
    [Space(10)]
    [Tooltip("The loudest the sound can randomly be.")]
    [Range(0, 1)]
    public float RandomVolumeMax = 1.0f;
    [Tooltip("The quietest the sound can randomly be.")]
    [Range(0, 1)]
    public float RandomVolumeMin = 1.0f;
    [Space(5)]
    [Tooltip("The highest pitch the sound can be.")]
    [Range(-3, 3)]
    public float RandomPitchMax = 1.0f;
    [Tooltip("The lowest pitch the sound can be.")]
    [Range(-3, 3)]
    public float RandomPitchMin = 1.0f;
    private float ranVol;
    private float ranPit;
    private int ranClip;
    private int lastRanClip;
    private bool stopPersistentSound = false;
    private List<int> ranList = new List<int>();

    public void PlaySound(int clipNum)
    {
        if (audioClips != null) {
            if (useClipsAsPlaylist == false)
            {
                if ((randomize == true || smartRandomize == true) && audioClips.Length > 1)
                {
                    PlaySoundRandomized();
                }
                else
                {
                    PlayOneSound(clipNum);
                }
            } else
            {
                PlayPersistentSounds();
            }
        } else
        {
            Debug.Log("You didn't set an audio clip in one of your audio scripts!");
        }
    }

    public void PlayOneSound(int clipNum)
    {
        audioPrefabClone = Instantiate(audioPrefab);
        if (audioPrefabParent != null)
        {
            audioPrefabClone.transform.SetParent(audioPrefabParent.transform, false);
        }
        AudioSource audioPrefabSource = audioPrefabClone.GetComponent<AudioSource>();
        audioPrefabSource.clip = audioClips[clipNum];
        audioPrefabSource.outputAudioMixerGroup = mixerTrack;
        if (RandomVolumeMax > RandomVolumeMin)
        {
            ranVol = Random.Range(RandomVolumeMin, RandomVolumeMax);
            audioPrefabSource.volume = ranVol;
        }
        else
        {
            audioPrefabSource.volume = StartVolume;
        }
        audioPrefabSource.panStereo = StereoPan;
        if (RandomPitchMax > RandomPitchMin)
        {
            ranPit = Random.Range(RandomPitchMin, RandomPitchMax);
            audioPrefabSource.pitch = ranPit;
        } else
        {
            audioPrefabSource.pitch = 1.0f;
        }
        audioPrefabSource.Play();
        if (loop)
        {
            audioPrefabSource.loop = true;
        }
        else
        {
            audioPrefabSource.loop = false;
        }
    }

    public void PlaySoundRandomized()
    {
        if (smartRandomize && audioClips.Length > 1)
        {
            ranClip = Random.Range(0, audioClips.Length);
            while (ranClip == lastRanClip)
            {
                ranClip = Random.Range(0, audioClips.Length);
            }
        } else
        {
            ranClip = Random.Range(0, audioClips.Length);
        }
        audioPrefabClone = Instantiate(audioPrefab);
        if (audioPrefabParent != null)
        {
            audioPrefabClone.transform.parent = audioPrefabParent.transform;
        }
        AudioSource audioPrefabSource = audioPrefabClone.GetComponent<AudioSource>();
        audioPrefabSource.clip = audioClips[ranClip];
        audioPrefabSource.outputAudioMixerGroup = mixerTrack;
        if (RandomVolumeMax > RandomVolumeMin)
        {
            ranVol = Random.Range(RandomVolumeMin, RandomVolumeMax);
            audioPrefabSource.volume = ranVol;
        }
        else
        {
            audioPrefabSource.volume = StartVolume;
        }
        audioPrefabSource.panStereo = StereoPan;
        if (RandomPitchMax > RandomPitchMin)
        {
            ranPit = Random.Range(RandomPitchMin, RandomPitchMax);
            audioPrefabSource.pitch = ranPit;
        }
        else
        {
            audioPrefabSource.pitch = 1.0f;
        }
        audioPrefabSource.Play();
        lastRanClip = ranClip;
        if (loop)
        {
            audioPrefabSource.loop = true;
        }
        else
        {
            audioPrefabSource.loop = false;
        }
    }

    public void PlayPersistentSounds()
    {
        audioPrefabClone = Instantiate(audioPrefab);
        if (audioPrefabParent != null)
        {
            audioPrefabClone.transform.parent = audioPrefabParent.transform;
        }
        AudioSource audioPrefabSource = audioPrefabClone.GetComponent<AudioSource>();
        audioPrefabSource.outputAudioMixerGroup = mixerTrack;
        audioPrefabSource.panStereo = StereoPan;
        audioPrefabSource.volume = StartVolume;
        audioPrefabClone.GetComponent<prefabAudioScript>().loop = loop;
        audioPrefabClone.GetComponent<prefabAudioScript>().RandomPitchMax = RandomPitchMax;
        audioPrefabClone.GetComponent<prefabAudioScript>().RandomPitchMin = RandomPitchMin;
        audioPrefabClone.GetComponent<prefabAudioScript>().RandomVolumeMax = RandomVolumeMax;
        audioPrefabClone.GetComponent<prefabAudioScript>().RandomVolumeMin = RandomVolumeMin;
        audioPrefabClone.GetComponent<prefabAudioScript>().persistentAudio = true;
        audioPrefabClone.GetComponent<prefabAudioScript>().stopSoundOnFadeOut = stopSoundOnFadeOut;
        audioPrefabClone.GetComponent<prefabAudioScript>().persistentSoundDelay = playlistClipDelay;
        if (randomize)
        {
            audioPrefabClone.GetComponent<prefabAudioScript>().randomize = true;
        }
        if (smartRandomize)
        {
            audioPrefabClone.GetComponent<prefabAudioScript>().smartRandomize = true;
        }
        audioPrefabClone.GetComponent<prefabAudioScript>().audioClips = audioClips;
    }

    public void FadeSoundIn()
    {
        if (audioPrefabClone != null)
        {
            audioPrefabClone.GetComponent<prefabAudioScript>().loop = loop;
            audioPrefabClone.GetComponent<prefabAudioScript>().FadeInSpeed = FadeInSpeed;
            audioPrefabClone.GetComponent<prefabAudioScript>().FadeInEndVolume = FadeInEndVolume;
            audioPrefabClone.GetComponent<prefabAudioScript>().FadeSoundIn();
        }
    }

    public void FadeSoundOut()
    {
        if (audioPrefabClone != null)
        {
            audioPrefabClone.GetComponent<prefabAudioScript>().FadeOutSpeed = FadeOutSpeed;
            audioPrefabClone.GetComponent<prefabAudioScript>().stopSoundOnFadeOut = stopSoundOnFadeOut;
            audioPrefabClone.GetComponent<prefabAudioScript>().FadeOutEndVolume = FadeOutEndVolume;
            audioPrefabClone.GetComponent<prefabAudioScript>().FadeSoundOut();
        }
    }

    public void StopSound()
    {
        GameObject soundObject = audioPrefabClone;
        AudioSource audioSource = audioPrefabClone.GetComponent<AudioSource>();
        if (letClipFinishOnStop == false)
        {
            if (useClipsAsPlaylist == true)
            {
                audioSource.Stop();
                audioPrefabClone.GetComponent<prefabAudioScript>().persistentAudio = false;
            }
            else
            {
                audioSource.Stop();
                Destroy(soundObject);
            }
        } else
        {
            if (useClipsAsPlaylist == true)
            {
                audioPrefabClone.GetComponent<prefabAudioScript>().persistentSoundOver = true;
            }
            else
            {
                audioSource.Stop();
                Destroy(soundObject);
            }
        }
    }
}