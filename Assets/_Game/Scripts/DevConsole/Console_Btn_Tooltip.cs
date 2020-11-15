using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Console_Button))]
public class Console_Btn_Tooltip : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] bool hasTooltip = true;
    [SerializeField] string toolTip = " ";
    [SerializeField] float popUpDelay = 0.5f;
    [Header("Visuals")]
    [SerializeField] GameObject toolTipPanel = null;
    [SerializeField] Console_Button button = null;

    private Coroutine runningRoutine;
    private IEnumerator displayCountdown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        OpenToolTip();
        StopCoroutine(runningRoutine);
    }

    #region Init

    private void Start()
    {
        if (!hasTooltip)
            return;

        if (button.hotkey != KeyCode.None)
            toolTip += " (" + button.hotkeyNickName + ")";

        Text _text = toolTipPanel.GetComponentInChildren<Text>();
        _text.text = toolTip;
    }

    private void OnEnable()
    {
        if(!hasTooltip)
            return;

        button.ButtonEnterHover += OnBeginHover;
        button.ButtonExitHover += OnEndHover;
        
    }

    private void OnDisable()
    {
        if (!hasTooltip)
            return;

        button.ButtonEnterHover += OnBeginHover;
        button.ButtonExitHover += OnEndHover;
        CloseToolTip();
        
    }

    #endregion

    private void OnBeginHover()
    {
        runningRoutine = StartCoroutine(displayCountdown(popUpDelay));
    }

    private void OnEndHover()
    {
        CloseToolTip();    
    }

    public void OpenToolTip()
    {
        if (!hasTooltip)
            return;

        if (runningRoutine != null)
            StopCoroutine(runningRoutine);
        toolTipPanel.SetActive(true);
    }

    public void CloseToolTip()
    {
        if (runningRoutine != null)
            StopCoroutine(runningRoutine);

        toolTipPanel.SetActive(false);
    }
}
