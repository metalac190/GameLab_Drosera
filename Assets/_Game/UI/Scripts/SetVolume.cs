using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class SetVolume : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] TextMeshProUGUI volumeText;

    public void SetVolumeLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        volumeText.text = (Mathf.RoundToInt(sliderValue * 100)).ToString();
    }
}
