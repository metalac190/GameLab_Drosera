using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip menuMusic, cutSceneMusic, jungleOne, jungleTwo, desertOne, desertTwo;

    private static MusicPlayer _instance = null;
    private AudioSource audioSource;
    private GameManager gm;

    public static MusicPlayer Instance
    {
        get
        {
            return MusicPlayer._instance;
        }
    }

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = menuMusic;
            audioSource.Play();

            gm = GameManager.Instance;
            gm.OnStateChange += HandleOnStateChange;
        }
    }

    public void HandleOnStateChange()
    {
        if(gm.GameState == DroseraGlobalEnums.GameState.CutScene)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
        else if (gm.GameState == DroseraGlobalEnums.GameState.MainOne)
        {
            if(gm.CurrentBiome == DroseraGlobalEnums.Biome.Desert)
            {
                audioSource.clip = desertOne;
                audioSource.Play();
            }
            else if (gm.CurrentBiome == DroseraGlobalEnums.Biome.Jungle)
            {
                audioSource.clip = jungleOne;
                audioSource.Play();
            }
            else //Not sure if not, so just play jungle for now.
            {
                audioSource.clip = jungleOne;
                audioSource.Play();
            }
        }
        else if (gm.GameState == DroseraGlobalEnums.GameState.MainTwo)
        {
            if (gm.CurrentBiome == DroseraGlobalEnums.Biome.Desert)
            {
                audioSource.clip = jungleTwo;
                audioSource.Play();
            }
            else if (gm.CurrentBiome == DroseraGlobalEnums.Biome.Jungle)
            {
                audioSource.clip = jungleTwo;
                audioSource.Play();
            }
            else
            {
                audioSource.clip = jungleTwo;
                audioSource.Play();
            }
        }
        else //Main menu
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
    }
}
