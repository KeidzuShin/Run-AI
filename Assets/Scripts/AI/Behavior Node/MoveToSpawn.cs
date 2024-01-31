using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MoveToSpawn : ActionNode
{
    float distanceToSpawn;
    protected override void OnStart() {
        context.agent.speed = blackboard.AISetting.moveSpeed;
        context.agent.angularSpeed = blackboard.AISetting.turnSpeed;
        context.agent.stoppingDistance = blackboard.AISetting.stoppingDistance;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        distanceToSpawn = Vector3.Distance(context.agent.transform.position, blackboard.spawn.transform.position);
        context.agent.SetDestination(blackboard.spawn.transform.position);
        if (distanceToSpawn < 5f)
            return State.Success;
        else
            return State.Running;
    }
}
