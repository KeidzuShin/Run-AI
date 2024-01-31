using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AIGameSetting", menuName = "ScriptableObject/AISetting")]
public class AIGameSetting : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 1f;
    public float fleeSpeed = 2f;
    public float turnSpeed = 200f;
    public float fleeturnSpeed = 360f;
    public float acceleration = 20f;
    public float stoppingDistance = 5f;

    [Header("Navmesh")]
    public float angleRange1 = 20f;
    public float angleRange2 = 160f;
    public float minFleeRange = 10f;
    public float maxFleeRange = 20f;
    public float maxFleeDistance = 30f;
}
