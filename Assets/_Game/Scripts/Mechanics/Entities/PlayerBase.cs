using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class PlayerBase : EntityBase
{
    //states
    protected enum PlayerState { Neutral, Attacking, Reloading, Ability, Dodging, Interacting, Dead };
    protected PlayerState currentState;

    //button variable names
    protected bool aimToggle;
    protected bool cycleTargetRight;
    protected bool cycleTargetLeft;
    protected bool reloadButton;
    protected bool abilityButton;
    protected bool interactButton;
    protected bool pauseButton;
    protected bool shootButtonKey;
    protected bool dodgeButtonKey;
    protected bool adjustCameraLeftKey;
    protected bool adjustCameraRightKey;
    //triggers and dpad are treated as axis
    protected float shootButtonGamepad;
    protected float dodgeButtonGamepad;
    protected float adjustCameraGamepad;
    //gunner specific
    protected bool altFireButton;
    protected bool swapAbilityButton;

    public bool AimToggle { get { return aimToggle; } }
    public bool CycleTargetRight { get { return cycleTargetRight; } }
    public bool CycleTargetLeft { get { return cycleTargetLeft; } }
    public bool AltFireButton { get { return altFireButton; } }

    protected Vector3 move;
    private CharacterController controller;

    [SerializeField]
    protected float playerY = .5f;

    [SerializeField]
    protected float abilityCooldownTime = 6.0f;
    protected float abilityCooldown = 0.0f;

    protected InteractableBase interactTarget;
    public InteractableBase InteractTarget { get { return interactTarget; } set { interactTarget = value; } }
    [SerializeField]
    protected float interactCooldown = 0.2f;
    protected float lastInteract = 0;

    [SerializeField]
    protected int ammo = 5;
    [SerializeField]
    protected int maxAmmo = 20;
    [SerializeField]
    protected int heldAmmo = 20;
    public int Ammo { get { return ammo; } set { ammo = value; } }
    [SerializeField]
    protected int ammoPerOre = 1;
    public int AmmoPerOre { get { return ammoPerOre; } }
    [SerializeField]
    protected float reloadCoolDownTime = 1.0f;
    protected float reloadCoolDown = 0f;

    new void Start()
    {
        base.Start();
        controller = gameObject.AddComponent<CharacterController>();
        currentState = PlayerState.Neutral;
    }

    public static PlayerBase instance;
    new void Awake()
    {
        base.Awake();
        instance = this;
    }

    // Update is called once per frame
    protected void Update()
    {
        //note: for dodge and shoot on controller need to use != 0

        if (currentState != PlayerState.Dead)
        {
            if (Input.GetJoystickNames().Length != 0) //controller or keyboard
            {
                //note: for dodge and shoot on controller need to use != 0
                aimToggle = Input.GetKeyDown(KeyCode.JoystickButton9) || Input.GetMouseButtonDown(2);
                cycleTargetRight = Input.GetAxis("Controller Right Stick X") > 0 || Input.mouseScrollDelta.y > 0;
                cycleTargetLeft = Input.GetAxis("Controller Right Stick X") < 0 || Input.mouseScrollDelta.y < 0;
                reloadButton = Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.R);
                abilityButton = Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.LeftShift);
                interactButton = Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.E);
                pauseButton = Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.Escape);
                dodgeButtonGamepad = Input.GetAxisRaw("Dodge");
                shootButtonGamepad = Input.GetAxisRaw("Shoot");
                adjustCameraGamepad = Input.GetAxisRaw("CameraAdjust");
                altFireButton = Input.GetKey(KeyCode.JoystickButton3) || Input.GetMouseButton(1);
                swapAbilityButton = Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.Q);
            }
            else //keyboard only
            {
                aimToggle = Input.GetMouseButtonDown(2);
                cycleTargetRight = Input.mouseScrollDelta.y > 0;
                cycleTargetLeft = Input.mouseScrollDelta.y < 0;
                reloadButton = Input.GetKey(KeyCode.R);
                abilityButton = Input.GetKey(KeyCode.LeftShift);
                interactButton = Input.GetKey(KeyCode.E);
                pauseButton = Input.GetKey(KeyCode.Escape);
                altFireButton = Input.GetMouseButton(1);
                swapAbilityButton = Input.GetKey(KeyCode.Q);
            }

            dodgeButtonKey = Input.GetKey(KeyCode.Space);
            shootButtonKey = Input.GetMouseButton(0);
            adjustCameraLeftKey = Input.GetKey(KeyCode.Z);
            adjustCameraRightKey = Input.GetKey(KeyCode.X);
        }
               

        //movement
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * _moveSpeed);
        if(transform.position.y != playerY)
        {
            transform.position = new Vector3(transform.position.x, playerY, transform.position.z);
        }
        if (move != Vector3.zero) //moving
        {

        }
        else if (currentState == PlayerState.Neutral) //idle
        {

        }

        //cooldowns
        abilityCooldown -= Time.deltaTime;
        reloadCoolDown -= Time.deltaTime;

        //states
        switch (currentState)
        {
            case PlayerState.Neutral: Neutral(); break;
            case PlayerState.Attacking: Attacking(); break;
            case PlayerState.Reloading: Reloading(); break;
            case PlayerState.Ability: Ability(); break;
            case PlayerState.Dodging: Dodging(); break;
            case PlayerState.Interacting: Interacting(); break;
            case PlayerState.Dead: Dead(); break;
            default: break;
        }

    }

    //states
    protected void Neutral()
    {
        if (shootButtonGamepad == 1 || shootButtonKey || altFireButton)
        {
            currentState = PlayerState.Attacking;
        }
        if (reloadButton)
        {
            currentState = PlayerState.Reloading;
        }
        if (abilityButton && abilityCooldown < 0.01)
        {
            currentState = PlayerState.Ability;
        }
        if (dodgeButtonGamepad == 1 || dodgeButtonKey)
        {
            currentState = PlayerState.Dodging;
        }
        if (interactButton && Time.fixedTime > lastInteract + interactCooldown)
        {
            currentState = PlayerState.Interacting;
        }
        interactTarget = null;
        if (_health <= 0)
        {
            currentState = PlayerState.Dead;
        }

    }

    protected virtual void Attacking()
    {
        if (ammo > 0) //have ammo
        {
            //attack stuff here
            currentState = PlayerState.Neutral;
        }
        else //no ammo
        {
            currentState = PlayerState.Reloading;
        }
    }

    protected void Reloading()
    {
        if (heldAmmo != 0 && reloadCoolDown < 0.01) //have ammo to reload and reload time is up
        {
            if (ammo != maxAmmo) //full
            {
                int tempAmmo = heldAmmo + ammo;
                if (tempAmmo > maxAmmo) //can't hold all the ammo
                {
                    ammo = maxAmmo;
                    heldAmmo = tempAmmo - maxAmmo;
                }
                else
                {
                    ammo = tempAmmo;
                    heldAmmo = 0;
                }
            }
            reloadCoolDown = reloadCoolDownTime;
        }
        currentState = PlayerState.Neutral;
    }

    protected virtual void Ability()
    {
        //ability stuff

        abilityCooldown = abilityCooldownTime;
        currentState = PlayerState.Neutral;
    }

    protected void Dodging()
    {
        currentState = PlayerState.Neutral;
    }

    protected void Interacting()
    {
        interactTarget?.Interact(this);
        lastInteract = Time.fixedTime;
        currentState = PlayerState.Neutral;
    }

    protected void Dead()
    {
        
    }
}
