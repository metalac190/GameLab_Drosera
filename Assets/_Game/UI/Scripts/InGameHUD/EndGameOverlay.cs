using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameOverlay : MonoBehaviour
{
    [SerializeField] float deathDisplayDelay = 1f;
    [SerializeField] float winDisplayDelay = 0;
    [SerializeField] GameObject deathOverlay;
    [SerializeField] GameObject winOverlay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateDeathScreen()
    {
        StartCoroutine(displayOverlay(deathDisplayDelay, deathOverlay));
    }

    public void ActivateWinScreen()
    {
        StartCoroutine(displayOverlay(winDisplayDelay, winOverlay));
    }

    private IEnumerator displayOverlay(float delay, GameObject overlay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(true);
        overlay.SetActive(true);
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
