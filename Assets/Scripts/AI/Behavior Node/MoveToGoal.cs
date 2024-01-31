using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MoveToGoal : ActionNode
{
    float distanceToGoal;
    protected override void OnStart() {
        context.agent.speed = blackboard.AISetting.moveSpeed;        
        context.agent.angularSpeed = blackboard.AISetting.turnSpeed;
        context.agent.stoppingDistance = blackboard.AISetting.stoppingDistance;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        distanceToGoal = Vector3.Distance(context.agent.transform.position, blackboard.finish.transform.position);
        context.agent.SetDestination(blackboard.finish.transform.position);
        if (distanceToGoal < 5f)
            return State.Success;     
        else     
            return State.Running;        
    }
}
