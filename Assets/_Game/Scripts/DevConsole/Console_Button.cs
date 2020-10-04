using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public enum eButtonState { neutralOff, hoverOff, depressedOff, depressedOn, hoverOn, neutralOn}

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class Console_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// William Austin
    /// A button class with edited functionality to swap sprites for each state
    /// </summary>
    
    public event Action ButtonEnterHover = delegate { };
    public event Action ButtonExitHover = delegate { };
    public UnityEvent ButtonActivated; 
    public UnityEvent ButtonDeactivated; 

    [SerializeField] public bool isToggle = false;
    [SerializeField] public bool deactivateOnEsc = true;

    [Header("Optional Hotkey")]
    [SerializeField] public KeyCode hotkey;
    [SerializeField] public string hotkeyNickName = "";
    public bool listenForHotkey {
        get { return _listenForHotkey; }
        set
        {
            if (hotkey == KeyCode.None)
                _listenForHotkey = false;
            else
            {
                _listenForHotkey = value;
            }
        }
    }
    private bool _listenForHotkey = false;

    [Header("Button Sprites")]
    [SerializeField] private Sprite spt_neutralOff = null;
    [SerializeField] private Sprite spt_hoverOff = null;
    [SerializeField] private Sprite spt_depressedOff = null;
    [SerializeField] private Sprite spt_depressedOn = null;
    [SerializeField] private Sprite spt_hoverOn = null;
    [SerializeField] private Sprite spt_neutralOn = null;

    [Header("Prefab References")]
    [SerializeField] GameObject hotkeyIcon = null;
    [SerializeField] private GameObject disabledButton = null;
    [SerializeField] private GameObject unimplementedButton = null;
    [SerializeField] private bool notImplemented = true;

    public eButtonState state { get => _state; private set { if (_state != value) { _state = value; OnStateChanged(); } } }
    private eButtonState _state = eButtonState.neutralOff;

    private Button _btn = null;
    private Image _btnImg = null;
    private Dictionary<eButtonState, Sprite> spriteBook;

    #region Init
    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btnImg = GetComponent<Image>();
    }

    private void Start()
    {
        spriteBook = new Dictionary<eButtonState, Sprite>(6);
        spriteBook.Add(eButtonState.neutralOff, spt_neutralOff);
        spriteBook.Add(eButtonState.hoverOff, spt_hoverOff);
        spriteBook.Add(eButtonState.depressedOff, spt_depressedOff);
        spriteBook.Add(eButtonState.depressedOn, spt_depressedOn);
        spriteBook.Add(eButtonState.hoverOn, spt_hoverOn);
        spriteBook.Add(eButtonState.neutralOn, spt_neutralOn);

        ChangeSprite();

        if (hotkey != KeyCode.None && hotkeyIcon != null)
        {
            hotkeyIcon.SetActive(true);
            Text childText = hotkeyIcon.GetComponentInChildren<Text>();
            childText.text = hotkeyNickName;
            if(CommandConsole.commandConsole.enableHotkeys)
                _listenForHotkey = true;
        }

        if (notImplemented)
            unimplementedButton?.SetActive(true);
    }

    private void OnEnable()
    {
        if(deactivateOnEsc)
            CommandConsole.RevertConsole += Deselect;
        CommandConsole.ToggleHotkeys += ToggleHotkey;
    }

    private void OnDisable()
    {
        if(deactivateOnEsc)
            CommandConsole.RevertConsole -= Deselect;
        CommandConsole.ToggleHotkeys -= ToggleHotkey;
    }

    #endregion

    private void Update()
    {
        if(listenForHotkey)
        {
            if(Input.GetKeyDown(hotkey))
            {
                ForceButtonClick();
            }
        }
    }

    #region Mouse Events
    /// <summary>
    /// Change the sprite of the button depending on the button state
    /// </summary>
    
    public void ForceButtonClick()
    {
        if (disabledButton.activeInHierarchy == true || notImplemented)
            return;

        _btn.onClick.Invoke();

        if (state == eButtonState.depressedOff || state == eButtonState.hoverOff || state == eButtonState.neutralOff)
        {
            ButtonActivated.Invoke();

            if (isToggle)
                state = eButtonState.neutralOn;
            else
            {
                ButtonDeactivated.Invoke();
                state = eButtonState.neutralOff;
            }
        }
        else if (state == eButtonState.depressedOn || state == eButtonState.hoverOn || state == eButtonState.neutralOn)
        {
            state = eButtonState.neutralOff;
            ButtonDeactivated.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (state == eButtonState.neutralOff)
            state = eButtonState.hoverOff;
        else if (state == eButtonState.neutralOn)
            state = eButtonState.hoverOn;

        ButtonEnterHover.Invoke();
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (state == eButtonState.hoverOff)
        {
            state = eButtonState.neutralOff;

        }
        else if (state == eButtonState.hoverOn)
        {
            state = eButtonState.neutralOn;
        }
        else if(state == eButtonState.depressedOff)
        {
            state = eButtonState.neutralOff;
        }
        else if(state == eButtonState.depressedOn)
        {
            state = eButtonState.neutralOn;
        }

        ButtonExitHover.Invoke();
    }

    public void OnPointerUp(PointerEventData data)
    {
        if(state == eButtonState.depressedOn)
        {
            state = eButtonState.hoverOff;
            ButtonDeactivated.Invoke();
        }
        else if(state == eButtonState.depressedOff)
        {
            ButtonActivated.Invoke();

            if (!isToggle)
            {
                state = eButtonState.hoverOff;
                ButtonDeactivated.Invoke();
            }
            else        
                state = eButtonState.hoverOn;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (state == eButtonState.hoverOn)
        {
            state = eButtonState.depressedOn;
        }
        else if (!isToggle || state == eButtonState.hoverOff)
        {
            state = eButtonState.depressedOff;
        }
    }
    #endregion
    private void OnStateChanged()
    {
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        _btnImg.sprite = spriteBook[state];
    }

    public void DisableButton()
    {
        disabledButton.SetActive(true);
    }

    private void ToggleHotkey(bool toState)
    {
        if (hotkey == KeyCode.None)
            return;

        listenForHotkey = toState;
    }

    private void Deselect()
    {
        if (state == eButtonState.depressedOn)
            state = eButtonState.depressedOff;
        else if (state == eButtonState.hoverOn)
            state = eButtonState.hoverOff;
        else if (state == eButtonState.neutralOn)
            state = eButtonState.neutralOff;

        ButtonDeactivated.Invoke();
    }

}
