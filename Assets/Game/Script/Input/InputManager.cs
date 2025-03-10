using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput; //Jump menggunakan Key 'Space'
    public Action OnClimbInput; //Climb menggunakan Key E
    public Action OnCancelInput; //Cancel climb menggunakan Key F
    public Action OnChangePOV; // Mengganti state kamera menggunakan Key C
    public Action OncrouchInput; //Crouch menggunakan Key LeftControl
    public Action OnGlideInput; //Glide menggunakan Key Q
    public Action OnCancelGlide; //Cancel glide menggunakan Key F
    public Action OnPunchInput; //Punch menggunakan Mouse kiri
    private void Update()
    {
        CheckMovementInput();
        CheckJumpInput();
        CheckSprintInput();
        CheckCrouchInput();
        CheckChangePOVInput();
        CheckClimbInput();
        CheckGlideInput();
        CheckCancelInput();
        CheckPunchInput();
        CheckMainMenuInput();
    }

    private void CheckMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        if (movement != null)
        {
            OnMoveInput(movement);
        }
    }

    private void CheckJumpInput()
    {        
        bool isJumping = Input.GetKeyDown(KeyCode.Space);
        if (isJumping)
        {
            if (OnJumpInput != null)
            OnJumpInput();
        }
    
    }

    private void CheckSprintInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            OnSprintInput(true);
        }
        else
        {
            OnSprintInput(false);
        }
    }

    private void CheckCrouchInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (OncrouchInput != null)
            OncrouchInput();
        }
    }

    private void CheckChangePOVInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (OnChangePOV != null)
            OnChangePOV();
        }
    }

    private void CheckClimbInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClimbInput();
        }
    }

    private void CheckGlideInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (OnGlideInput != null)
            OnGlideInput();
        }
    }

    private void CheckCancelInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (OnCancelInput != null)
            OnCancelInput();
            
            if (OnCancelGlide != null)
            OnCancelGlide();
        }

    }

    private void CheckPunchInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (OnPunchInput != null)
            OnPunchInput();
        }
    }

    private void CheckMainMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Main Menu");
        }
    }
}
