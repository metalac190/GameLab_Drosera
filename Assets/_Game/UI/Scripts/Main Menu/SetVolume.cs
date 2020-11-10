using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SetVolume : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeText;

    public void SetVolumeLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        volumeSlider.value = sliderValue;
        volumeText.text = (Mathf.RoundToInt(sliderValue * 100)).ToString();
    }
}
