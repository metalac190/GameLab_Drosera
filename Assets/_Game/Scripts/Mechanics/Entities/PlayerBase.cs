using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBase : EntityBase
{
    //button variable names
    protected bool aimToggle;
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

    protected Vector3 move;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //assign buttons
        if (Input.GetJoystickNames().Length != 0) //controller
        {
            //note: for dodge and shoot on controller need to use != 0
            aimToggle = Input.GetKey(KeyCode.JoystickButton9);
            reloadButton = Input.GetKey(KeyCode.JoystickButton2);
            abilityButton = Input.GetKey(KeyCode.JoystickButton4);
            interactButton = Input.GetKey(KeyCode.JoystickButton1);
            pauseButton = Input.GetKey(KeyCode.JoystickButton7);
            dodgeButtonGamepad = Input.GetAxisRaw("9");
            shootButtonGamepad = Input.GetAxisRaw("10");
            adjustCameraGamepad = Input.GetAxisRaw("6");
            altFireButton = Input.GetKey(KeyCode.JoystickButton3);
            swapAbilityButton = Input.GetKey(KeyCode.JoystickButton5);
        }
        else //keyboard
        {
            aimToggle = Input.GetMouseButton(3);
            reloadButton = Input.GetKey(KeyCode.R);
            abilityButton = Input.GetKey(KeyCode.LeftShift);
            interactButton = Input.GetKey(KeyCode.E);
            pauseButton = Input.GetKey(KeyCode.Escape);
            dodgeButtonKey = Input.GetKey(KeyCode.Space);
            shootButtonKey = Input.GetMouseButton(1);
            adjustCameraLeftKey = Input.GetKey(KeyCode.Z);
            adjustCameraRightKey = Input.GetKey(KeyCode.X);
            altFireButton = Input.GetMouseButton(2);
            swapAbilityButton = Input.GetKey(KeyCode.Q);
        }

        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    }
}
