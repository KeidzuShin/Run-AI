using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.Burst.CompilerServices;
using UnityEngine.AI;

[System.Serializable]
public class IsWaypointNear : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        float distanceToGoal = Vector3.Distance(context.agent.transform.position, blackboard.finish.transform.position);
        
        if (distanceToGoal <= context.gameObject.GetComponent<AIStats>().searchRange * 2)
            return State.Success;
        else
            return State.Failure;

        #region Advanced (W.I.P)
        /*
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(context.agent.transform.position, blackboard.finish.transform.position, NavMesh.AllAreas, path);
        float pathLength = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
            pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);

        if(pathLength > blackboard.AISetting.maxFleeDistance * 2)
            return State.Success;
        else
            return State.Failure;
        */
        #endregion
    }
}
