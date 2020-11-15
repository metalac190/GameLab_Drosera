using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] GameObject[] selectedStuff;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeText;
    float volume;

    int currentlySelected;
    int previouslySelected;

    MenuManager menuManager;
    SetVolume setVolume;

    PauseController pauseController;

    // controller support
    bool axisInUse = false;
    bool cycleDown;
    bool cycleUp;
    bool cycleLeft;
    bool cycleRight;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
        setVolume = FindObjectOfType<SetVolume>();

        pauseController = FindObjectOfType<PauseController>();

        HighlightElement(1);
    }

    // Update is called once per frame
    void Update()
    {
        ControllerSupport();
    }

    void ControllerSupport()
    {
        if (Input.GetJoystickNames().Length != 0)
        {
            ChangeSelection();
            AdjustSlider();

            // confirm (A)
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                if (currentlySelected == 0)
                {
                    menuManager.DisplayMainMenuPanel();
                    menuManager.PlaySound(0);
                }
            }
            // go back (B)
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                menuManager.DisplayMainMenuPanel();
                menuManager.PlaySound(0);
            }
        }
    }

    void HighlightElement(int index)
    {
        previouslySelected = currentlySelected;
        currentlySelected = index;

        // glow on back button
        if (currentlySelected == 0)
        {
            volumeText.outlineWidth = 0;

            if (selectedStuff[index] != null)
                selectedStuff[index].SetActive(true);
        }
        // highlight volume text;
        else if (currentlySelected == 1)
        {
            if (selectedStuff[0] != null)
                selectedStuff[0].SetActive(false);

            volumeText.outlineWidth = 0.2f;
        }
    }

    void AdjustSlider()
    {
        if (currentlySelected == 1)
        {
            cycleRight = Input.GetAxisRaw("Horizontal") == 1;
            cycleLeft = Input.GetAxisRaw("Horizontal") == -1;

            volume = volumeSlider.value;

            if (cycleLeft)
            {
                volume -= Time.deltaTime;
                if (volume < 0)
                    volume = 0.0001f;
                setVolume.SetVolumeLevel(volume);
            }
            if (cycleRight)
            {
                volume += Time.deltaTime;
                if (volume > 1)
                    volume = 1;
                setVolume.SetVolumeLevel(volume);
            }
        }
    }

    void ChangeSelection()
    {
        cycleUp = Input.GetAxisRaw("Vertical") == 1;
        cycleDown = Input.GetAxisRaw("Vertical") == -1;

        if (pauseController == null)
        {
            if (cycleUp)
            {
                HighlightElement(1);
            }
            if (cycleDown)
            {
                HighlightElement(0);
            }
        }
    }
}
