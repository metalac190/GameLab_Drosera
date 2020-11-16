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

    [SerializeField] Image leftPanel;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI paragraphTextLeft;
    [SerializeField] TextMeshProUGUI paragraphTextRight;
    [SerializeField] TextMeshProUGUI extraNotesTop;
    [SerializeField] TextMeshProUGUI extraNotesDown;
    [SerializeField] Image image1;
    [SerializeField] Image image2;
    [SerializeField] Image image3;   // top right (layout 2)

    [SerializeField] GameObject[] crewHoverLines;
    [SerializeField] GameObject[] docsHoverLines;
    [SerializeField] GameObject[] mailHoverLines;
    [SerializeField] GameObject[] mediaHoverLines;

    [Header("Codex Entries Back End")]
    [SerializeField] CodexEntry[] crewEntries;
    [SerializeField] CodexEntry[] docEntries;
    [SerializeField] CodexEntry[] mailEntries;
    [SerializeField] CodexEntry[] mediaEntries;

    [SerializeField] Sprite[] leftPanelSprites;

    public string currentSection;
    public int currentEntry;
    public int previousEntry;
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
        previousEntry = currentEntry;
        currentEntry = num;

        ClearLayout();

        // check button section
        switch (currentSection)
        {
            // show correct layout and information
            case "Crew":
                crewHoverLines[currentEntry].SetActive(true);
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
                docsHoverLines[currentEntry].SetActive(true);
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
                mailHoverLines[currentEntry].SetActive(true);
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
                mediaHoverLines[currentEntry].SetActive(true);
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

        if (menuManager != null)
            menuManager.PlaySound(1);
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

        if (entry.imageTopRight != null)
            image3.sprite = entry.imageTopRight;

        image3.CrossFadeAlpha(1, 0, false);
    }

    void ShowEntryLayout3(CodexEntry3 entry)
    {
        titleText.text = entry.codexTitle;
        if (entry.image != null)
            image1.sprite = entry.image;
        if (entry.extraNotesTop != null)
            extraNotesTop.text = entry.extraNotesTop;

        image1.CrossFadeAlpha(1, 0, false);
    }

    void ShowEntryLayout4(CodexEntry4 entry)
    {
        titleText.text = entry.codexTitle;

        if (entry.imageLeft != null)
            image1.sprite = entry.imageLeft;
        if (entry.imageRight != null)
            image2.sprite = entry.imageRight;
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
        image3.CrossFadeAlpha(0, 0, false);

        foreach (GameObject obj in crewHoverLines)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in docsHoverLines)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in mailHoverLines)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in mediaHoverLines)
        {
            obj.SetActive(false);
        }
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

        if (menuManager != null)
            menuManager.PlaySound(1);

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

        if (menuManager != null)
            menuManager.PlaySound(1);

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

        leftPanel.sprite = leftPanelSprites[0];
        // leftPanel.SetNativeSize();

        totalEntries = crewParentObj.transform.childCount;

        if (menuManager != null && this.gameObject.activeInHierarchy)
            menuManager.PlaySound(1);

        ShowCurrentEntry(0);
    }

    public void DocsButton()
    {
        TurnOffEntryList();
        docsParentObj.SetActive(true);
        currentSection = "Docs";

        leftPanel.sprite = leftPanelSprites[1];
        // leftPanel.SetNativeSize();

        totalEntries = docsParentObj.transform.childCount;

        if (menuManager != null)
            menuManager.PlaySound(1);

        ShowCurrentEntry(0);
    }

    public void MailButton()
    {
        TurnOffEntryList();
        mailParentObj.SetActive(true);
        currentSection = "Mail";

        leftPanel.sprite = leftPanelSprites[2];
        // leftPanel.SetNativeSize();

        totalEntries = mailParentObj.transform.childCount;

        if (menuManager != null)
            menuManager.PlaySound(1);

        ShowCurrentEntry(0);
    }

    public void MediaButton()
    {
        TurnOffEntryList();
        mediaParentObj.SetActive(true);
        currentSection = "Media";

        leftPanel.sprite = leftPanelSprites[3];
        // leftPanel.SetNativeSize();

        totalEntries = mailParentObj.transform.childCount;

        if (menuManager != null)
            menuManager.PlaySound(1);

        ShowCurrentEntry(0);
    }
}
