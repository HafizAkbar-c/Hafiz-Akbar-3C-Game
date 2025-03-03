using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput;
    public Action OnClimbInput;
    public Action OnCancelInput;
    public Action OnChangePOV; // Mengganti state kamera menggunakan Key C
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
            Debug.Log("Crouch");
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
            Debug.Log("Glide");
        }
    }

    private void CheckCancelInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (OnCancelInput != null)
            OnCancelInput();
        }
    }

    private void CheckPunchInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Punch");
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
