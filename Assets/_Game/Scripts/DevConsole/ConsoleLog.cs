using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleLog : MonoBehaviour
{
    [SerializeField] private GameObject display = null;
    [SerializeField] private GameObject closedDisplay = null;
    [SerializeField] private GameObject logParent = null;
    [SerializeField] private Text gameLog = null;
        private string output;
        private string stack;

    private bool isVisible = false;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

    private void Start()
    {
        isVisible = false;
        display?.SetActive(false);
        closedDisplay?.SetActive(true);
        SetLogState(true);
    }

    public void OpenLog()
    {
        SetLogState(true);
    }

    public void CloseLog()
    {
        SetLogState(false);
    }

    public void ShowLog()
    {
        SetLogVisibility(true);
    }

    public void HideLog()
    {
        SetLogVisibility(false);
    }

    private void SetLogVisibility(bool toState)
    {
        if (isVisible == toState)
            return;

        isVisible = toState;
        display?.SetActive(toState);
        closedDisplay?.SetActive(!toState);
    }

    private void SetLogState(bool toState)
    {
        logParent.SetActive(toState);
    }

    public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            gameLog.text = output + "\n" + gameLog.text;
            if (gameLog.text.Length > 5000)
            {
                gameLog.text = gameLog.text.Substring(0, 4000);
            }
        }

}

