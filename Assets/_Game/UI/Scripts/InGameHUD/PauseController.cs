using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseController : MonoBehaviour
{
    [Header("HUDs")]
    [SerializeField] GameObject resumeHUD;
    [SerializeField] GameObject extrasHUD;
    [SerializeField] GameObject settingsHUD;
    [SerializeField] GameObject quitHUD;

    [Header("Assets")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeText;
    [SerializeField] Image menuText;
    [SerializeField] Image quitText;

    int currentlySelectedElement;
    public int CurrentlySelectedElement { get => currentlySelectedElement; set => currentlySelectedElement = value; }

    Pause pause;
    AudioScript audioScript;

    SetVolume setVolume;
    float volume;
    bool cycleRight;
    bool cycleLeft;

    private void Start()
    {
        pause = FindObjectOfType<Pause>();
        audioScript = GetComponent<AudioScript>();
    }

    private void Update()
    {
        if (Input.GetJoystickNames().Length != 0)
        {
            if (resumeHUD.activeInHierarchy)
            {
                ControllerSupportForResume();
            }
            else if (extrasHUD.activeInHierarchy)
            {
                ControllerSupportForExtras();
            }
            else if (settingsHUD.activeInHierarchy)
            {
                ControllerSupportForSettings();
            }
            else if (quitHUD.activeInHierarchy)
            {
                ControllerSupportForQuit();
            }
        }
    }

    void ControllerSupportForResume()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            pause.ResumeGame();
        }
    }

    void ControllerSupportForExtras()
    {

    }

    void ControllerSupportForSettings()
    {
        AdjustSlider();
    }

    void AdjustSlider()
    {
        if (setVolume == null)
            setVolume = volumeSlider.GetComponent<SetVolume>();

        volumeText.outlineWidth = 0.2f;

        if (currentlySelectedElement == 0)
        {
            cycleRight = Input.GetAxisRaw("Horizontal") == 1;
            cycleLeft = Input.GetAxisRaw("Horizontal") == -1;

            volume = volumeSlider.value;

            if (cycleLeft)
            {
                volume -= Time.unscaledDeltaTime;
                if (volume < 0)
                    volume = 0.0001f;
                setVolume.SetVolumeLevel(volume);
            }
            if (cycleRight)
            {
                volume += Time.unscaledDeltaTime;
                if (volume > 1)
                    volume = 1;
                setVolume.SetVolumeLevel(volume);
            }
        }
    }

    void ControllerSupportForQuit()
    {
        cycleRight = Input.GetAxisRaw("Horizontal") == 1;
        cycleLeft = Input.GetAxisRaw("Horizontal") == -1;

        if (cycleRight)
        {
            currentlySelectedElement = 1;
            ChangeQuitTextColor(false);
        }
        if (cycleLeft)
        {
            currentlySelectedElement = 0;
            ChangeQuitTextColor(true);
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            if (currentlySelectedElement == 0)
            {
                pause.BackToMenu();
            }
            else if (currentlySelectedElement == 1)
            {
                pause.QuitGame();
            }
        }
    }

    public void ChangeQuitTextColor(bool menu)
    {
        if (menu)
        {
            quitText.color = Color.white;
            menuText.color = Color.yellow;
        }
        else
        {
            quitText.color = Color.yellow;
            menuText.color = Color.white;
        }
    }
}
