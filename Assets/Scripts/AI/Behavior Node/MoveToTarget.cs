using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MoveToTarget : ActionNode
{
    [Header("Set Target")]
    public bool Goal;
    public bool Spawn;
    public bool Player;
    float distanceToGoal;
    float distanceToSpawn;
    float distanceToPlayer;
    protected override void OnStart()
    {
        context.agent.speed = blackboard.AISetting.moveSpeed;
        context.agent.angularSpeed = blackboard.AISetting.turnSpeed;
        context.agent.stoppingDistance = blackboard.AISetting.stoppingDistance;
    }

    protected override void OnStop(){
    }

    protected override State OnUpdate()
    {
        if (Goal)
        {
            moveToGoal();
            if (distanceToGoal < 5f)
                return State.Success;
            else
                return State.Running;
        }
        else if (Spawn)
        {
            moveToSpawn();
            if (distanceToSpawn < 5f)
                return State.Success;
            else
                return State.Running;
        }
        else if (Player)
        {
            moveToPlayer();
            if (distanceToPlayer < 5f)
                return State.Success;
            else
                return State.Running;
        }
        else
        {
            return State.Success;
        }
    }

    public void moveToGoal()
    {
        distanceToGoal = Vector3.Distance(context.agent.transform.position, blackboard.finish.transform.position);
        context.agent.SetDestination(blackboard.finish.transform.position);
    }
    public void moveToSpawn()
    {
        distanceToSpawn = Vector3.Distance(context.agent.transform.position, blackboard.spawn.transform.position);
        context.agent.SetDestination(blackboard.spawn.transform.position);
    }
    public void moveToPlayer()
    {
        distanceToPlayer = Vector3.Distance(context.agent.transform.position, blackboard.player.transform.position);
        context.agent.SetDestination(blackboard.player.transform.position);
    }
}
