using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementAndJump : MonoBehaviour
{
    [Header("References")]
    public GameSetting GameSetting;

    bool readytoJump = true;
    bool isGrounded = true;
    bool isSprinting = false;

    Cinemachine.CinemachineVirtualCameraBase VirtualCam;
    private Rigidbody Rigidbody;
    private float turnSpeedMultiplier;
    private float speed = 0f;
    private Animator animator;
    private Vector3 targetDirection;
    private Vector2 XYinput;
    private Quaternion freeRotation;
    private Camera mainCamera;
    private float velocity;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        VirtualCam = FindAnyObjectByType<Cinemachine.CinemachineVirtualCameraBase>();        
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GetInput();
        //StateHandler();
        //SpeedControl();
        Animation();
        Text();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Check if player isGrounded
        isGrounded = Physics.Raycast(Rigidbody.position, Vector3.down, GameSetting.playerHeight * 0.5f + 0.2f, GameSetting.WhatIsGround);

        #region Ground Drag
        if (isGrounded) Rigidbody.drag = GameSetting.GroundDrag;
        else Rigidbody.drag = 0;
        #endregion

        MovePlayer();
        GravityControl();

        // set sprinting
        isSprinting = ((Input.GetKey(GameSetting.sprintKey)) && XYinput != Vector2.zero);

        // Update target direction relative to the camera view (or not if the Keep Direction option is checked)
        UpdateTargetDirection();        
        if (XYinput != Vector2.zero && targetDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = targetDirection.normalized;
            freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
            var eulerY = transform.eulerAngles.y;

            if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = freeRotation.eulerAngles.y;
            var euler = new Vector3(0, eulerY, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), GameSetting.turnSpeed * turnSpeedMultiplier * Time.deltaTime);
        }
        
        //Save Scriptable Object so it can be used during build
        JsonUtility.ToJson(GameSetting);
    }

    public virtual void UpdateTargetDirection()
    {
        turnSpeedMultiplier = 1f;
        var forward = mainCamera.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        //get the right-facing direction of the referenceTransform
        var right = mainCamera.transform.TransformDirection(Vector3.right);

        // determine the direction the player will face based on input and the referenceTransform's right and forward directions
        targetDirection = XYinput.x * right + XYinput.y * forward;
    }

    private void GetInput()
    {
        #region Jump / Double Jump
        //Wait for input to execute jump
        if (Input.GetKey(GameSetting.jumpKey) && readytoJump)
        {
            if (isGrounded)
            {
                Jump(1);
                Invoke(nameof(ResetJump), GameSetting.jumpCooldown);
            }
            else
                Jump(2);
        }
        //When character is on a ground, it can jump once more.
        if (isGrounded) ResetJump();
        #endregion

        #region Crouching
        if (Input.GetMouseButtonDown(0) && Input.GetKeyDown(GameSetting.crouchKey))
        {
            Rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        #endregion
    }

    private void MovePlayer()
    {
        XYinput.x = Input.GetAxis("Horizontal");
        XYinput.y = Input.GetAxis("Vertical");
        speed = Mathf.Abs(XYinput.x) + Mathf.Abs(XYinput.y);

        if (isSprinting)
            speed = Mathf.Clamp(speed, 0f, 1f);
        else
            speed = Mathf.Clamp(speed, 0f, 0.5f);

        speed = Mathf.SmoothDamp(animator.GetFloat("Speed"), speed, ref velocity, GameSetting.SmoothTime);
        animator.SetFloat("Speed", speed);

        #region Move With RigidBody or Velocity Method
        //Rigidbody.AddForce(moveDir.normalized * movespeed * 10f, ForceMode.Force);        
        //Rigidbody.velocity = new Vector3(moveDir.x * Movement.movespeed, Rigidbody.velocity.y, moveDir.z * Movement.movespeed);
        #endregion
    }

    #region Jump Function
    private void Jump(float flag)
    {
        Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0f, Rigidbody.velocity.z);
        if (flag == 1)
            Rigidbody.AddForce(transform.up * GameSetting.jumpforce, ForceMode.Impulse);
        else
        {
            Rigidbody.AddForce(transform.up * GameSetting.doublejumpforce, ForceMode.Impulse);
            readytoJump = false;
        }
        animator.SetBool("isJumping", true);
    }

    private void ResetJump()
    {
        readytoJump = true;
    }
    #endregion

    private void GravityControl()
    {
        //Incrementally increase gravity until hit ground or max gravity then return it to normal
        if (!isGrounded && Physics.gravity.y > GameSetting.maxGravity)
            Physics.gravity += GameSetting.gravityIncreaseRate * Physics.gravity.normalized * Time.fixedDeltaTime;
        else
            Physics.gravity = new Vector3(0, -9.81f, 0);
    }

    private void Animation()
    {
        #region Jump / Fall Animation
        if (isGrounded)
        {
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
        else
        {
            animator.SetBool("isGrounded", false);
            if (Rigidbody.velocity.y != 0)
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
            }
        }
        #endregion
    }

    /*
    private void StateHandler()
    {
        //Sprinting
        if (Input.GetKey(GameSetting.sprintKey))
        {
            state = MovementState.sprinting;
            movespeed = GameSetting.sprintSpeed;
            //SmoothCameraFOV(2);
        }

        //Crouching
        else if (Input.GetKey(GameSetting.crouchKey))
        {
            state = MovementState.crouching;
            movespeed = GameSetting.crouchSpeed;
        }

        //Walking
        else if (isGrounded || Input.GetKeyUp(GameSetting.sprintKey))
        {
            state = MovementState.walking;
            movespeed = GameSetting.movespeed;
            //SmoothCameraFOV(1);
        }

        //Air
        else
        {
            state = MovementState.air;
            movespeed = GameSetting.movespeed * GameSetting.airMultiplier;
        }
    }
    */

    [Header("Text")]
    public TextMeshProUGUI text_speed;
    private void Text()
    {
        Vector3 flatVel = new Vector3(Rigidbody.velocity.x, 0f, Rigidbody.velocity.z);

        text_speed.SetText("Move Speed: " + Round(Rigidbody.velocity.magnitude, 1));
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
}


