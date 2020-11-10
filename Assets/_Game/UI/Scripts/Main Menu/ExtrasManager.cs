using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasManager : MonoBehaviour
{
    [SerializeField] GameObject[] selectedStuff;
    int currentlySelected;
    int previouslySelected;

    MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();

        HighlightElement(0);
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

        selectedStuff[index].SetActive(true);
    }
}
