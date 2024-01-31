using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class PlayerRigidMovement : MonoBehaviour
{
    [Header("References")]
    public Transform CameraOrientation;    
    public Transform PlayerContainer;
    public Transform PlayerObject;    
    public GameSetting GameSetting;
    public GrapplingGun grappling;
    //public CinemachineFreeLook Cinemachine;
    public CinemachineVirtualCameraBase VirtualCam;
    public Rigidbody Rigidbody;
    public Animator animator;
    public Camera mainCamera;

    bool readytoJump = true;
    bool isGrounded = true;

    private Vector2 XYinput;
    private Vector3 targetDirection;
    private Quaternion freeRotation;
    private float movespeed;
    private Vector3 strafeDirection = Vector3.zero;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        swinging,
        crouching,
        air
    }

    void Start()
    {
        #region Find References
        if (!animator) animator = GetComponentInChildren<Animator>();
        animator.SetBool("isMoving", false);

        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.freezeRotation = true;

        mainCamera = Camera.main;
        VirtualCam = FindAnyObjectByType<CinemachineVirtualCameraBase>();

        GameSetting = Resources.Load<GameSetting>("GameSetting");
        #endregion

        readytoJump = true;
    }

    void Update()
    {
        //Check if player isGrounded
        isGrounded = Physics.Raycast(Rigidbody.position, Vector3.down, GameSetting.playerHeight * 0.5f + 0.2f, GameSetting.WhatIsGround);
        //if (isGrounded) Debug.Log("Grounded");

        #region Ground Drag
        if (isGrounded) Rigidbody.drag = GameSetting.GroundDrag;
        else Rigidbody.drag = 0;
        #endregion

        GetInput();
        StateHandler();
        SpeedControl();
        Animation();
        Text();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        GravityControl();

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

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), GameSetting.turnSpeed * GameSetting.turnSpeedMultiplier * Time.deltaTime);
        }

        //Save Scriptable Object so it can be used during build
        JsonUtility.ToJson(GameSetting);
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
        if (state == MovementState.swinging && Input.GetKeyDown(GameSetting.crouchKey))
        {
            Rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        #endregion
    }

    private void StateHandler()
    {
        //Grappling
        if (grappling.IsGrappling() && !isGrounded)
        {
            state = MovementState.swinging;
            movespeed = Mathf.MoveTowards(movespeed, GameSetting.swingSpeed, GameSetting.moveSpeedMultiplier * Time.deltaTime);                
        }

        //Sprinting
        else if (Input.GetKey(GameSetting.sprintKey))
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
            movespeed = Mathf.MoveTowards(movespeed, GameSetting.movespeed * GameSetting.airMultiplier, GameSetting.moveSpeedMultiplier * Time.deltaTime);            
        }
    }

    private void GravityControl()
    {
        //Incrementally increase gravity until hit ground or max gravity then return it to normal
        if (state == MovementState.swinging)
            Physics.gravity = new Vector3(0, Mathf.MoveTowards(Physics.gravity.y, GameSetting.maxGravity, GameSetting.gravityIncreaseRate * Time.deltaTime), 0);
        else
            Physics.gravity = new Vector3(0, -9.81f, 0);
    }

    private void MovePlayer()
    {
        // Get the forward vector of the camera
        Vector3 cameraForward = mainCamera.transform.forward;

        // Ignore the y-axis rotation
        cameraForward.y = 0f;
        cameraForward = cameraForward.normalized;

        // Calculate the movement direction relative to the camera's forward direction
        Vector3 moveDir = Input.GetAxisRaw("Horizontal") * mainCamera.transform.right + Input.GetAxisRaw("Vertical") * cameraForward;

        // Normalize the movement direction to ensure consistent movement speed
        moveDir = moveDir.normalized;        

        // Rotate the player to face the movement direction
        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.15f);
        }

        Rigidbody.AddForce(moveDir.normalized * movespeed * 10f, ForceMode.Force);
        //Rigidbody.AddForce(moveDir.normalized * movespeed, ForceMode.Impulse);

        #region Move with Velocity Method
        //Rigidbody.velocity = new Vector3(moveDir.x * Movement.movespeed, Rigidbody.velocity.y, moveDir.z * Movement.movespeed);

        //Rigidbody.MovePosition(transform.position + moveDir * GameSetting.movespeed * Time.fixedDeltaTime);
        #endregion

    }

    public virtual void UpdateTargetDirection()
    {
        var forward = mainCamera.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        //get the right-facing direction of the referenceTransform
        var right = mainCamera.transform.TransformDirection(Vector3.right);

        // determine the direction the player will face based on input and the referenceTransform's right and forward directions
        targetDirection = XYinput.x * right + XYinput.y * forward;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(Rigidbody.velocity.x, 0f, Rigidbody.velocity.z);

        //Limit velocity
        //if (flatVel.magnitude > movespeed && state == MovementState.swinging)
        if (state == MovementState.swinging)
        {
            if(flatVel.magnitude > GameSetting.sprintSpeed * 2.5f) { 
                Vector3 limitedVel = flatVel.normalized * GameSetting.sprintSpeed * 2.5f;
                Rigidbody.velocity = new Vector3(limitedVel.x, Rigidbody.velocity.y, limitedVel.z);
            }
            else
            {
                Rigidbody.velocity = new Vector3(
                    Mathf.MoveTowards(Rigidbody.velocity.x, 0f, GameSetting.GroundDrag / 10f * Time.deltaTime),
                    Rigidbody.velocity.y,
                    Mathf.MoveTowards(Rigidbody.velocity.z, 0f, GameSetting.GroundDrag / 10f * Time.deltaTime)
                );
            }
            
        }
        else if (flatVel.magnitude > movespeed)
        {
            Vector3 limitedVel = flatVel.normalized * movespeed;
            Rigidbody.velocity = new Vector3(
                Mathf.MoveTowards(Rigidbody.velocity.x, limitedVel.x, GameSetting.GroundDrag * 10f * Time.deltaTime),                
                Rigidbody.velocity.y,
                Mathf.MoveTowards(Rigidbody.velocity.z, limitedVel.z, GameSetting.GroundDrag * 10f * Time.deltaTime)
            );
        }        
    }

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

    #region Smooth Camera FOV using FreeLook Cinemachine
    /*
    private void SmoothCameraFOV(float x)
    {
        if (x == 1)
            if (Cinemachine.m_Lens.FieldOfView > GameSetting.WalkFOV)            
                Cinemachine.m_Lens.FieldOfView -= GameSetting.SmoothTransition * 10f * Time.deltaTime;

        if (x == 2)
            if (Cinemachine.m_Lens.FieldOfView < GameSetting.SprintFOV)
                Cinemachine.m_Lens.FieldOfView += GameSetting.SmoothTransition * 10f * Time.deltaTime;
    }
    */
    #endregion

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
            if(Rigidbody.velocity.y != 0)
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
            }            
        }
        #endregion

        #region Walk / Sprint Animation
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            animator.SetBool("isMoving", true);
            if (Input.GetKey(GameSetting.sprintKey))
                animator.SetFloat("Vertical", 1f, GameSetting.WalkDelta, Time.deltaTime);
                //animator.SetFloat("Vertical", Mathf.MoveTowards(animator.GetFloat("Vertical"), 1f, GameSetting.WalkDelta * Time.deltaTime));
            else
                animator.SetFloat("Vertical", 0.3f, GameSetting.WalkDelta, Time.deltaTime);
                //animator.SetFloat("Vertical", Mathf.MoveTowards(animator.GetFloat("Vertical"), 0.3f, GameSetting.WalkDelta * Time.deltaTime));
        }
        else
        {
            animator.SetFloat("Vertical", 0f, GameSetting.WalkDelta, Time.deltaTime);
            //animator.SetFloat("Vertical", Mathf.MoveTowards(animator.GetFloat("Vertical"), 0f, GameSetting.WalkDelta * Time.deltaTime));

            if(animator.GetFloat("Vertical") == 0)
                animator.SetBool("isMoving", false);
        }
        #endregion
        //animator.SetFloat("Horizontal", Mathf.MoveTowards(animator.GetFloat("Horizontal"), 1, GameSetting.maxDelta * Time.deltaTime));

        #region Dead Animation
        float currenthealth = gameObject.GetComponent<PlayerStats>().currentHealth;
        if (currenthealth <= 0)
        {
            animator.SetBool("Dead", true);
        }            
        #endregion
    }

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
