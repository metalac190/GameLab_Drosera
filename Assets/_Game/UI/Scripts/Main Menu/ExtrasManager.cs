using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExtrasManager : MonoBehaviour
{
    [SerializeField] GameObject[] selectedStuff;
    int currentlySelected;
    int previouslySelected;

    [Header("Codex Entries Front End")]
    [SerializeField] GameObject crewParentObj;
    [SerializeField] GameObject docsParentObj;
    [SerializeField] GameObject mailParentObj;
    [SerializeField] GameObject mediaParentObj;

    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI paragraphTextLeft;
    [SerializeField] TextMeshProUGUI paragraphTextRight;
    [SerializeField] TextMeshProUGUI extraNotesTop;
    [SerializeField] TextMeshProUGUI extraNotesDown;
    [SerializeField] Image image1;
    [SerializeField] Image image2;

    [Header("Codex Entries Back End")]
    [SerializeField] CodexEntry[] crewEntries;
    [SerializeField] CodexEntry[] docEntries;
    [SerializeField] CodexEntry[] mailEntries;
    [SerializeField] CodexEntry[] mediaEntries;

    public string currentSection;
    public int currentEntry;
    public int totalEntries;

    MenuManager menuManager;

    // Start is called before the first frame update
    void Start()
    {
        CrewButton();

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

    // get entry index
    public void ShowCurrentEntry(int num)
    {
        currentEntry = num;

        ClearLayout();

        // check button section
        switch (currentSection)
        {
            // show correct layout and information
            case "Crew":
                if (crewEntries[currentEntry].layout == 1)
                {
                    ShowEntryLayout1((CodexEntry1)crewEntries[num]);
                }
                else if (crewEntries[currentEntry].layout == 2)
                {
                    ShowEntryLayout2((CodexEntry2)crewEntries[num]);
                }
                else if (crewEntries[currentEntry].layout == 3)
                {
                    ShowEntryLayout3((CodexEntry3)crewEntries[num]);
                }
                else if (crewEntries[currentEntry].layout == 4)
                {
                    ShowEntryLayout4((CodexEntry4)crewEntries[num]);
                }
                break;
            case "Docs":
                if (docEntries[currentEntry].layout == 1)
                {
                    ShowEntryLayout1((CodexEntry1)docEntries[num]);
                }
                else if (docEntries[currentEntry].layout == 2)
                {
                    ShowEntryLayout2((CodexEntry2)docEntries[num]);
                }
                else if (docEntries[currentEntry].layout == 3)
                {
                    ShowEntryLayout3((CodexEntry3)docEntries[num]);
                }
                else if (docEntries[currentEntry].layout == 4)
                {
                    ShowEntryLayout4((CodexEntry4)docEntries[num]);
                }
                break;
            case "Mail":
                if (mailEntries[currentEntry].layout == 1)
                {
                    ShowEntryLayout1((CodexEntry1)mailEntries[num]);
                }
                else if (mailEntries[currentEntry].layout == 2)
                {
                    ShowEntryLayout2((CodexEntry2)mailEntries[num]);
                }
                else if (mailEntries[currentEntry].layout == 3)
                {
                    ShowEntryLayout3((CodexEntry3)mailEntries[num]);
                }
                else if (mailEntries[currentEntry].layout == 4)
                {
                    ShowEntryLayout4((CodexEntry4)mailEntries[num]);
                }
                break;
            case "Media":
                if (mediaEntries[currentEntry].layout == 1)
                {
                    ShowEntryLayout1((CodexEntry1)mediaEntries[num]);
                }
                else if (mediaEntries[currentEntry].layout == 2)
                {
                    ShowEntryLayout2((CodexEntry2)mediaEntries[num]);
                }
                else if (mediaEntries[currentEntry].layout == 3)
                {
                    ShowEntryLayout3((CodexEntry3)mediaEntries[num]);
                }
                else if (mediaEntries[currentEntry].layout == 4)
                {
                    ShowEntryLayout4((CodexEntry4)mediaEntries[num]);
                }
                break;
        }
    }

    void ShowEntryLayout1(CodexEntry1 entry)
    {
        titleText.text = entry.codexTitle;
        if (entry.paragraphTextLeft != null)
            paragraphTextLeft.text = entry.paragraphTextLeft;
        if (entry.paragraphTextRight != null)
            paragraphTextRight.text = entry.paragraphTextRight;
    }

    void ShowEntryLayout2(CodexEntry2 entry)
    {
        titleText.text = entry.codexTitle;
        if (entry.paragraphTextLeft != null)
            paragraphTextLeft.text = entry.paragraphTextLeft;
        if (entry.extraNotesBottom != null)
            extraNotesDown.text = entry.extraNotesBottom;
    }

    void ShowEntryLayout3(CodexEntry3 entry)
    {
        titleText.text = entry.codexTitle;
        if (entry.image != null)
            image1 = entry.image;
        if (entry.extraNotesTop != null)
            extraNotesTop.text = entry.extraNotesTop;

        image1.CrossFadeAlpha(1, 0, false);
    }

    void ShowEntryLayout4(CodexEntry4 entry)
    {
        titleText.text = entry.codexTitle;

        if (entry.imageLeft != null)
            image1 = entry.imageLeft;
        if (entry.imageRight != null)
            image2 = entry.imageRight;
        if (entry.extraNotesTop != null)
            extraNotesTop.text = entry.extraNotesTop;

        image1.CrossFadeAlpha(1, 0, false);
        image2.CrossFadeAlpha(1, 0, false);
    }

    void ClearLayout()
    {
        titleText.text = "";
        paragraphTextLeft.text = "";
        paragraphTextRight.text = "";
        extraNotesTop.text = "";
        extraNotesDown.text = "";
        image1.CrossFadeAlpha(0, 0, false);
        image2.CrossFadeAlpha(0, 0, false);
    }

    public void RightArrow()
    {
        switch (currentSection)
        {
            case "Crew":
                totalEntries = crewParentObj.transform.childCount;
                break;
            case "Docs":
                totalEntries = docsParentObj.transform.childCount;
                break;
            case "Mail":
                totalEntries = mailParentObj.transform.childCount;
                break;
            case "Media":
                totalEntries = mediaParentObj.transform.childCount;
                break;
        }

        if (currentEntry + 1 < totalEntries)
        {
            currentEntry++;
        }

        ShowCurrentEntry(currentEntry);
    }

    public void LeftArrow()
    {
        switch (currentSection)
        {
            case "Crew":
                totalEntries = crewParentObj.transform.childCount;
                break;
            case "Docs":
                totalEntries = docsParentObj.transform.childCount;
                break;
            case "Mail":
                totalEntries = mailParentObj.transform.childCount;
                break;
            case "Media":
                totalEntries = mediaParentObj.transform.childCount;
                break;
        }

        if (currentEntry - 1 >= 0)
        {
            currentEntry--;
        }

        ShowCurrentEntry(currentEntry);
    }

    void TurnOffEntryList()
    {
        crewParentObj.SetActive(false);
        docsParentObj.SetActive(false);
        mailParentObj.SetActive(false);
        mediaParentObj.SetActive(false);
    }

    public void CrewButton()
    {
        TurnOffEntryList();
        crewParentObj.SetActive(true);
        currentSection = "Crew";

        ShowCurrentEntry(0);
    }

    public void DocsButton()
    {
        TurnOffEntryList();
        docsParentObj.SetActive(true);
        currentSection = "Docs";

        ShowCurrentEntry(0);
    }

    public void MailButton()
    {
        TurnOffEntryList();
        mailParentObj.SetActive(true);
        currentSection = "Mail";

        ShowCurrentEntry(0);
    }

    public void MediaButton()
    {
        TurnOffEntryList();
        mediaParentObj.SetActive(true);
        currentSection = "Media";

        ShowCurrentEntry(0);
    }
}
