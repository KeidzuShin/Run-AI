using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "ScriptableObject/Setting")]
public class GameSetting : ScriptableObject
{
    [Header("Basic Movement")]
    public float movespeed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float swingSpeed = 1f;
    public float turnSpeed = 7f;
    public float turnSpeedMultiplier = 1f;
    public float moveSpeedMultiplier = 1f;

    [Header("Jumping and Air")]
    public float jumpforce = 8f;
    public float doublejumpforce = 5f;
    public float jumpCooldown = 0.5f;
    public float airMultiplier = 1.1f;

    [Header("Ground Check")]
    public float playerHeight = 2;
    public float GroundDrag = 5;
    public LayerMask WhatIsGround;
    public LayerMask whatIsGrappleable;

    [Header("Cinemachine")]
    public float WalkFOV = 50;
    public float SprintFOV = 55;
    public float SmoothTransition = 3f;

    [Header("Animation")]
    public float WalkDelta = 0.1f;
    public float SmoothTime = 0.3f;

    [Header("Gravity")]
    public float maxGravity = -10f;
    public float gravityIncreaseRate = 2f;

    [Header("Spring Joint")]
    public float maxDistance = 100f;
    public float Spring = 2f;
    public float SpringDamper = 1f;
    public float SpringMass = 10f;

    [Header("Player Base Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100f;
    public float lowHealth = 50f;
    public float regenRate = 5f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    public int storeElapsedTime = 0;
}
