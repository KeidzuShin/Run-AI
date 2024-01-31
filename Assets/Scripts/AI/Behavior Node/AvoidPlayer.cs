using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class AvoidPlayer : ActionNode
{
    public bool isDestinationReached = false;
    public bool hasRunPath = false;
    Vector3 destination;
    private Vector3 previousPosition;

    protected override void OnStart() {
        context.agent.speed = blackboard.AISetting.fleeSpeed;
        context.agent.angularSpeed = blackboard.AISetting.fleeturnSpeed;
        context.agent.stoppingDistance = 0;
        previousPosition = context.agent.transform.position;
    }

    protected override void OnStop() {
        hasRunPath = false;
        context.agent.stoppingDistance = 1;
    }

    protected override State OnUpdate() {
        float distanceToPlayer = Vector3.Distance(context.agent.transform.position, blackboard.player.transform.position);
        //Check if the agent is approaching the player.

        if (distanceToPlayer < context.gameObject.GetComponent<AIStats>().searchRange && distanceToPlayer < Vector3.Distance(previousPosition, blackboard.player.transform.position))
        {//Calculate the destination point based on the player's forward direction.
            Vector3 PlayerForward = blackboard.player.transform.position + blackboard.player.transform.forward * 20f;
            NavMeshHit hit;

            //Check if destination is within Navmesh area
            if (NavMesh.SamplePosition(PlayerForward, out hit, blackboard.AISetting.maxFleeRange, NavMesh.AllAreas))
            {
                context.agent.SetDestination(hit.position);
                destination = hit.position;
            }

            else
                SetRandomDestination();
        }
        else
        {   //Redo finding path if NULL
            if (!hasRunPath)
            {
                SetRandomDestination();
            }
        }

        //Blackboard
        previousPosition = context.agent.transform.position;
        float distanceToDestination = Vector3.Distance(context.agent.transform.position, destination);

        if (distanceToDestination > 3)
            return State.Running;
        else
            return State.Success;
    }

    void SetRandomDestination()
    {        
        Vector3 directionToPlayer = context.agent.transform.position - blackboard.player.transform.position;
        Vector3 perpendicularDirection = Vector3.Cross(directionToPlayer, Vector3.up); // Get perpendicular direction from the player
        Vector3 randomDirection = Quaternion.AngleAxis(Random.Range(blackboard.AISetting.angleRange1, blackboard.AISetting.angleRange2), Vector3.up) * perpendicularDirection.normalized; // Add random angle to the direction
        Vector3 randomPosition = context.agent.transform.position + randomDirection * Random.Range(blackboard.AISetting.minFleeRange, blackboard.AISetting.maxFleeRange); // Add random distance
        NavMeshHit hit;

        //Check if destination is within Navmesh area
        if (NavMesh.SamplePosition(randomPosition, out hit, blackboard.AISetting.maxFleeRange, NavMesh.AllAreas))
        {
            #region Prevent navmesh path too far             
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(context.agent.transform.position, hit.position, NavMesh.AllAreas, path))
            {// Calculate the length of the path
                float pathLength = 0f;
                for (int i = 1; i < path.corners.Length; i++)
                    pathLength += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                
                if (path.status == NavMeshPathStatus.PathComplete && pathLength > blackboard.AISetting.maxFleeDistance)
                {// Check if the path length is greater than the maximum allowed distance
                    Vector3 newTargetPosition = context.agent.transform.position + (randomPosition - context.agent.transform.position).normalized * blackboard.AISetting.maxFleeRange;
                    if (NavMesh.SamplePosition(newTargetPosition, out hit, blackboard.AISetting.maxFleeRange, NavMesh.AllAreas))
                    {
                        context.agent.SetDestination(hit.position);
                        destination = hit.position;
                        hasRunPath = true;
                    }
                }
                #endregion

                else
                {
                    // Set the agent's destination to the original target position
                    context.agent.SetDestination(hit.position);
                    destination = hit.position;
                    hasRunPath = true;
                }
            }
        }
    }
}