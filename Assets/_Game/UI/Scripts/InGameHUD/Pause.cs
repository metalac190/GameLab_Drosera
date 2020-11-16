using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [Header("Pause Panels")]
    [SerializeField] GameObject playerHUD;
    [SerializeField] GameObject inGameHUD;
    [SerializeField] GameObject pauseHUD;
    [SerializeField] GameObject pauseBackgroundImage;
    [SerializeField] GameObject[] panels;
    bool isPaused = false;

    [Header("Pause Texts")]
    [SerializeField] Image[] pauseTexts;
    [SerializeField] Sprite[] normalTexts;
    [SerializeField] Sprite[] coloredTexts;
    int currentlySelected = 0;
    int previouslySelected = -1;

    bool axisInUse;
    bool cycleRight;
    bool cycleLeft;

    PauseController pauseController;
    AudioScript audioScript;
    [SerializeField] ExtrasManager extrasManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        ControllerSupport();
    }

    void ControllerSupport()
    {
        if (Input.GetJoystickNames().Length != 0 && isPaused)
        {
            cycleRight = Input.GetKeyDown(KeyCode.Joystick1Button5);
            cycleLeft = Input.GetKeyDown(KeyCode.Joystick1Button4);

            if (cycleRight && !axisInUse)
            {
                if (currentlySelected + 1 <= 4)
                    SwitchToPausePanel(currentlySelected + 1);

                axisInUse = true;
                StartCoroutine(ControllerAxisCooldown());
            }
            if (cycleLeft && !axisInUse)
            {
                if (currentlySelected - 1 >= 0)
                    SwitchToPausePanel(currentlySelected - 1);

                axisInUse = true;
                StartCoroutine(ControllerAxisCooldown());
            }
        }
    }

    IEnumerator ControllerAxisCooldown()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        axisInUse = false;
    }

    public void PauseGame()
    {
        playerHUD.SetActive(false);
        inGameHUD.SetActive(false);

        Time.timeScale = 0;

        pauseHUD.SetActive(true);

        pauseController = pauseHUD.GetComponent<PauseController>();
        audioScript = pauseHUD.GetComponent<AudioScript>();

        SwitchToPausePanel(0);
    }

    // switch to selected pause panel and highlight respective pause text
    public void SwitchToPausePanel(int index)
    {
        if (index != currentlySelected)
        {
            pauseController.CurrentlySelectedElement = 0;

            previouslySelected = currentlySelected;
            currentlySelected = index;

            pauseTexts[previouslySelected].sprite = normalTexts[previouslySelected];
            pauseTexts[previouslySelected].SetNativeSize();

            pauseTexts[currentlySelected].sprite = coloredTexts[currentlySelected];
            pauseTexts[currentlySelected].SetNativeSize();

            panels[currentlySelected].SetActive(true);
            panels[previouslySelected].SetActive(false);

            // extras
            if (currentlySelected == 2)
            {
                extrasManager.CrewButton();
                pauseBackgroundImage.SetActive(false);
            }
            else
            {
                pauseBackgroundImage.SetActive(true);
            }

            audioScript.PlayOneSound(0);
        }
    }

    public void ResumeGame()
    {
        playerHUD.SetActive(true);
        inGameHUD.SetActive(true);

        Time.timeScale = 1;

        pauseHUD.SetActive(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.GameState = DroseraGlobalEnums.GameState.Menu;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
