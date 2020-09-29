using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTemp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        GameManager.Instance.GameState = DroseraGlobalEnums.GameState.Menu;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerBase>() && GameManager.Instance.GameState == DroseraGlobalEnums.GameState.MainTwo)
        {
            BackToMenu();
        }
    }
}
