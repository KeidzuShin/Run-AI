using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(NavMeshAgent))]

public class AiGoToTarget1 : MonoBehaviour
{
    [Header("References")]    
    public Camera cam;
    public Transform target;
    public NavMeshAgent agent;
    public ThirdPersonCharacter character;
    public AIGameSetting AISetting;

    float timer = 0.0f;
    public enum TargetMode {
        PointCamera, FollowPlayer
    }

    public TargetMode targetMode;


    void Start()
    {
        #region Find References
        cam = Camera.main;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (!target) target = GameObject.FindGameObjectWithTag("Player").transform;
        if (!character) ThirdPersonCharacter.Destroy(character);
        character = GetComponent<ThirdPersonCharacter>();
        #endregion

        agent.updateRotation = false;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && targetMode == TargetMode.PointCamera)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
        else if (targetMode == TargetMode.FollowPlayer)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                agent.destination = target.position;
                timer = 0.5f;
            }

            agent.speed = AISetting.moveSpeed;
            agent.angularSpeed = AISetting.turnSpeed;
            agent.acceleration = AISetting.acceleration;
            agent.stoppingDistance = AISetting.stoppingDistance;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            //character.Move(Vector3 move, bool crouch, bool jump;
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }
}
