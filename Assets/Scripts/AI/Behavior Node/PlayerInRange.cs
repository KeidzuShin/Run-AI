using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class PlayerInRange : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        float distanceToPlayer = Vector3.Distance(context.agent.transform.position, blackboard.player.transform.position);
        blackboard.distanceToPlayer = distanceToPlayer;
        if (distanceToPlayer <= context.gameObject.GetComponent<AIStats>().searchRange)         
            return State.Success;            
        else
            return State.Failure;            
    }
}
