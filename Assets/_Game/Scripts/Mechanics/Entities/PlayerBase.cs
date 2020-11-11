using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Playables;

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
    public bool ReloadButton { get { return reloadButton; } }
    protected bool abilityButton;
    public bool AbilityButton { get { return abilityButton; } }
    protected bool interactButton;
    protected bool pauseButton;
    public bool PauseButton { get { return pauseButton; } }
    protected bool shootButtonKey;
    protected bool dodgeButtonKey;
    public bool DodgeButtonKey { get { return dodgeButtonKey; } }
    protected bool adjustCameraLeftKey;
    protected bool adjustCameraRightKey;
    //triggers and dpad are treated as axis
    protected float shootButtonGamepad;
    protected float dodgeButtonGamepad;
    protected float adjustCameraGamepad;
    //gunner specific
    protected bool altFireButton;
    protected bool swapAbilityButton;
    public bool SwapAbilityButton { get { return swapAbilityButton; } }

    public bool AimToggle { get { return aimToggle; } }
    public bool CycleTargetRight { get { return cycleTargetRight; } }
    public bool CycleTargetLeft { get { return cycleTargetLeft; } }
    public bool AltFireButton { get { return altFireButton; } }
    public bool AdjustCameraRight { get { return adjustCameraRightKey; } }
    public bool AdjustCameraLeft { get { return adjustCameraLeftKey; } }

    private CharacterController controller;

    protected Vector3 xMove;
    protected Vector3 zMove;
    protected Vector3 movement;

    [SerializeField]
    protected float playerY = .5f;

    [SerializeField]
    protected float dodgeCooldownTime = 2.0f;
    public float DodgeCooldownTime { get { return dodgeCooldownTime; } }
    protected float dodgeCooldown = 0.0f;
    public float DodgeCooldown { get { return dodgeCooldown; } }
    [SerializeField]
    protected float dodgeSpeed = 100;
    [SerializeField]
    protected float dodgeTime = .2f;
    public float DodgeTime { get { return dodgeTime; } }
    protected float dodgeTimer = 0.0f;

    [SerializeField]
    protected float altFireCooldownTime = 6.0f;
    public float AltFireCooldownTime { get { return altFireCooldownTime; } }
    protected float altFireCooldown = 0.0f;

    protected InteractableBase interactTarget;
    public InteractableBase InteractTarget { get { return interactTarget; } set { interactTarget = value; } }
    [SerializeField]
    protected float interactCooldown = 0.2f;
    protected float lastInteract = 0;

    [SerializeField]
    GameObject _projectile;

    Transform _gunEnd;

    [SerializeField]
    protected int ammo = 5;
    [SerializeField]
    protected int maxAmmo = 20;
    [SerializeField]
    protected int heldAmmo = 20;
    public int Ammo { get { return ammo; } set { ammo = value; } }
    public int HeldAmmo { get { return heldAmmo; } set { heldAmmo = value; } }
    public int MaxAmmo { get { return maxAmmo; } set { maxAmmo = value; } }
    [SerializeField]
    protected int ammoPerOre = 1;
    public int AmmoPerOre { get { return ammoPerOre; } }
    [SerializeField]
    protected float reloadCoolDownTime = 1.0f;
    protected float reloadCoolDown = 0f;

    [SerializeField]
    protected float lowHealthPercentage = .3f;

    [SerializeField]
    GameObject dodgeVFX;
    GameObject tempDVFX;

    [SerializeField]
    protected float lowHealthSoundDelay = .6f;

    protected bool lowHealthPlaying = false;

    [SerializeField]
    protected float iFrameRate = .1f;

    public UnityEvent OnReload;
    public UnityEvent OnDodge;
    public UnityEvent OnLowHealth;

    // Room detection/reference for enemies
    public Room currentRoom;

    protected override void Start()
    {
        base.Start();
        controller = gameObject.GetComponent<CharacterController>();
        currentState = PlayerState.Neutral;
    }

    public static PlayerBase instance;
    protected override void Awake()
    {
        base.Awake();

        _gunEnd = transform.GetChild(0).transform;
        _animator = GetComponent<Animator>();

        Physics.IgnoreLayerCollision(11, 16);
        Physics.IgnoreLayerCollision(16, 15);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //note: for dodge and shoot on controller need to use != 0

        if (currentState != PlayerState.Dead && GameManager.Instance.GameState != DroseraGlobalEnums.GameState.CutScene && Time.timeScale > 0)
        {
            if (Input.GetJoystickNames().Length != 0) //controller or keyboard
            {
                //note: for dodge and shoot on controller need to use != 0
                aimToggle = Input.GetKeyDown(KeyCode.JoystickButton9) || Input.GetMouseButtonDown(2);
                cycleTargetRight = Input.GetAxis("Controller Right Stick X") > 0 || Input.mouseScrollDelta.y > 0;
                cycleTargetLeft = Input.GetAxis("Controller Right Stick X") < 0 || Input.mouseScrollDelta.y < 0;
                reloadButton = Input.GetKey(KeyCode.JoystickButton2) || Input.GetKey(KeyCode.R);
                abilityButton = Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.LeftShift);
                interactButton = Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.E);
                pauseButton = Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape);
                dodgeButtonKey = Input.GetKeyDown(KeyCode.Space);
                dodgeButtonGamepad = Input.GetAxisRaw("Dodge");
                shootButtonGamepad = Input.GetAxisRaw("Shoot");
                adjustCameraGamepad = Input.GetAxisRaw("CameraAdjust");
                altFireButton = Input.GetKey(KeyCode.JoystickButton5) || Input.GetMouseButton(1);
                swapAbilityButton = Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.Q);
            }
            else //keyboard only
            {
                aimToggle = Input.GetMouseButtonDown(2);
                cycleTargetRight = Input.mouseScrollDelta.y > 0;
                cycleTargetLeft = Input.mouseScrollDelta.y < 0;
                reloadButton = Input.GetKey(KeyCode.R);
                abilityButton = Input.GetKey(KeyCode.LeftShift);
                interactButton = Input.GetKey(KeyCode.E);
                pauseButton = Input.GetKeyDown(KeyCode.Escape);
                dodgeButtonKey = Input.GetKeyDown(KeyCode.Space);
                altFireButton = Input.GetMouseButton(1);
                swapAbilityButton = Input.GetKeyDown(KeyCode.Q);
            }

            dodgeButtonKey = Input.GetKey(KeyCode.Space);
            shootButtonKey = Input.GetMouseButton(0);
            adjustCameraLeftKey = Input.GetKey(KeyCode.Z);
            adjustCameraRightKey = Input.GetKey(KeyCode.C);
        }

        

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight = new Vector3(cameraRight.x, 0, cameraRight.z).normalized;

        //movement
        zMove = Input.GetAxis("Vertical") * cameraForward;
        xMove = Input.GetAxis("Horizontal") * cameraRight;

        if (currentState != PlayerState.Dodging)
        {
            movement = (zMove + xMove).normalized * Mathf.Max(zMove.magnitude, xMove.magnitude);
            controller.Move(movement * Time.deltaTime * _moveSpeed);
        }
        else
        {
            RaycastHit hit;
            if (movement != Vector3.zero)
            {
                Debug.DrawRay(transform.position + new Vector3(0, 0.4f, 0), movement.normalized * Time.deltaTime * dodgeSpeed, Color.cyan);
                if (Physics.Raycast(transform.position + new Vector3(0, 0.4f, 0), movement, out hit, (movement.normalized * Time.deltaTime * dodgeSpeed).magnitude))
                {
                    if (hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 0)
                    {
                        controller.Move(movement.normalized * hit.distance);
                    }
                    else
                    {
                        controller.Move(movement.normalized * Time.deltaTime * dodgeSpeed);
                    }
                }
                else
                {
                    controller.Move(movement.normalized * Time.deltaTime * dodgeSpeed);
                }
            }
            else
            {
                Debug.DrawRay(transform.position + new Vector3(0, 0.4f, 0), transform.forward * Time.deltaTime * dodgeSpeed, Color.cyan);
                if (Physics.Raycast(transform.position + new Vector3(0, 0.4f, 0), transform.forward, out hit, (transform.forward * Time.deltaTime * dodgeSpeed).magnitude))
                {
                    if (hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 0)
                    {
                        controller.Move(transform.forward * hit.distance);
                    }
                    else
                    {
                        controller.Move(transform.forward * Time.deltaTime * dodgeSpeed);
                    }
                }
                else
                {
                    controller.Move(transform.forward * Time.deltaTime * dodgeSpeed);
                }
            }
        }
        
        if (transform.position.y != playerY)
        {
            transform.position = new Vector3(transform.position.x, playerY, transform.position.z);
        }

        //animation variables
        float facing = Vector3.SignedAngle(transform.forward, movement, transform.up);
        if (movement != Vector3.zero) //moving
        {
            //walking
            if (Mathf.Abs(facing) <= 45) //forward
            {
                _animator.SetInteger("walkAni", 1);
                //Debug.Log("F");
            }
            if (facing > 45 && facing <= 135) //right
            {
                _animator.SetInteger("walkAni", 3);
                //Debug.Log("R");
            }
            if (Mathf.Abs(facing) > 135) //back
            {
                _animator.SetInteger("walkAni", 2);
                //Debug.Log("B");
            }
            if (facing < -45 && facing >= -135) //left
            {
                _animator.SetInteger("walkAni", 4);
                //Debug.Log("L");
            }

            //shoot
            if(currentState == PlayerState.Attacking)
            {
                _animator.SetInteger("shootAni", 2);
            }

            //dodging
            if (currentState == PlayerState.Dodging)
            {
                if (Mathf.Abs(facing) <= 45) //forward
                {
                    _animator.SetInteger("dodgeAni", 1);
                    //Debug.Log("F");
                }
                if (facing > 45 && facing <= 135) //right
                {
                    _animator.SetInteger("dodgeAni", 3);
                    //Debug.Log("R");
                }
                if (Mathf.Abs(facing) > 135) //back
                {
                    _animator.SetInteger("dodgeAni", 2);
                    //Debug.Log("B");
                }
                if (facing < -45 && facing >= -135) //left
                {
                    _animator.SetInteger("dodgeAni", 4);
                    //Debug.Log("L");
                }
            }
            else
            {
                _animator.SetInteger("dodgeAni", 0);
            }
        }
        else if (currentState == PlayerState.Neutral) //idle
        {
            _animator.SetInteger("walkAni", 0);
        }
        else if (currentState == PlayerState.Attacking)
        {
            _animator.SetInteger("shootAni", 1);
        }
        else if (currentState == PlayerState.Dodging)
        {
            _animator.SetInteger("dodgeAni", 1);
            //Debug.Log("F");
        }
        else
        {
            _animator.SetInteger("dodgeAni", 0);
        }

        if(_health/_maxHealth < lowHealthPercentage && !lowHealthPlaying) //low health
        {
            StartCoroutine("LowHealth");
        }

        //cooldowns
        altFireCooldown -= Time.deltaTime;
        dodgeCooldown -= Time.deltaTime;

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
        _animator.SetInteger("shootAni", 0);
        _animator.SetBool("grenadeAni", false);
        _animator.SetBool("getHyperSeedAni", false);
        _animator.SetBool("getAmmoAni", false);
        if (shootButtonGamepad == 1 || shootButtonKey || altFireButton)
        {
            currentState = PlayerState.Attacking;
        }
        if (reloadButton && heldAmmo != 0)
        {
            currentState = PlayerState.Reloading;
        }
        if (abilityButton)
        {
            currentState = PlayerState.Ability;
        }
        if ((dodgeButtonGamepad == 1 || dodgeButtonKey) && dodgeCooldown < 0.01)
        {
            OnDodge?.Invoke();
            tempDVFX = Instantiate(dodgeVFX, transform.position, Quaternion.identity);
            ParticleSystem part = tempDVFX.GetComponent<ParticleSystem>();
            part.Play();
            currentState = PlayerState.Dodging;
        }
        if (interactButton && Time.fixedTime > lastInteract + interactCooldown)
        {
            currentState = PlayerState.Interacting;
        }
        
        if (_health <= 0)
        {
            currentState = PlayerState.Dead;
        }
        dodgeTimer = 0;
        reloadCoolDown = 0;
    }

    protected virtual void Attacking()
    {

        if (ammo > 0) //have ammo
        {
            //attack stuff here
            
            Instantiate(_projectile, _gunEnd.position, _gunEnd.rotation);
            currentState = PlayerState.Neutral;
        }
        else //no ammo
        {
            currentState = PlayerState.Reloading;
        }
    }

    protected void Reloading()
    {
        if (reloadCoolDown < 0.01 && heldAmmo > 0) //have ammo to reload and reload time is up
        {     
            if (ammo != maxAmmo) //full
            {
                _animator.SetBool("reloadAni", true);
                OnReload?.Invoke();
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
        }
        if(reloadCoolDown<reloadCoolDownTime)
        {
            reloadCoolDown += Time.deltaTime;
        }
        else
        {
            _animator.SetBool("reloadAni", false);
            currentState = PlayerState.Neutral;
        }
        
    }

    protected virtual void Ability()
    {
        //ability stuff

        currentState = PlayerState.Neutral;
    }

    protected void Dodging()
    {
        tempDVFX.transform.position = transform.position;
        tempDVFX.transform.rotation = transform.rotation;
        if (dodgeTimer < dodgeTime)
        {
            GetComponentInChildren<TrailRenderer>().emitting = true;
            _isInvincible = true;
            dodgeTimer += Time.deltaTime;
        }
        else
        {
            GetComponentInChildren<TrailRenderer>().emitting = false;
            _isInvincible = false;
            Destroy(tempDVFX, dodgeCooldownTime);
            dodgeCooldown = dodgeCooldownTime;
            currentState = PlayerState.Neutral;
        }
        
    }

    protected void Interacting()
    {
        interactTarget?.Interact(this);
        if(interactTarget.GetComponent<HyperSeed>() != null)
        {
            _animator.SetBool("getHyperSeedAni", true);
        }
        else if(interactTarget.GetComponent<OreVein>() != null)
        {
            _animator.SetBool("getAmmoAni", true);
        }
        interactTarget = null;
        lastInteract = Time.fixedTime;
        currentState = PlayerState.Neutral;
    }

    protected void Dead()
    {
        //dead sound
        _animator.SetBool("deathAni", true);
        GameManager.Instance.GameLost();
        Debug.Log("You are dead.");
    }

    public override void TakeDamage(float value)
    {
        _health -= value;
        OnTakeDamage?.Invoke();
        if (_health <= 0)
        {
            currentState = PlayerState.Dead;
        }
        else
        {
            _animator.SetBool("damageAni", true);
        }

        StartCoroutine("InvincibleAfterDmg");
    }

    IEnumerator LowHealth()
    {
        while(_health/_maxHealth < lowHealthPercentage)
        {
            lowHealthPlaying = true;
            OnLowHealth?.Invoke();
            yield return new WaitForSeconds(lowHealthSoundDelay);
        }
        lowHealthPlaying = false;
    }

    IEnumerator InvincibleAfterDmg()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(iFrameRate);
        _animator.SetBool("damageAni", false);
        _isInvincible = false;
    }
}
