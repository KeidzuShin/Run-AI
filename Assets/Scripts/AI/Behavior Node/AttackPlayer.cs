using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;
using System.Threading;

[System.Serializable]
public class AttackPlayer : ActionNode
{
    bool alreadyAttacked = false;
    float flag;
    public AIStats AIStats;
    Timer timer;
    Transform pivot; // Empty GameObject as the rotation pivot
    Transform attackPoint;

    protected override void OnStart() {
        if (!attackPoint)
            attackPoint = context.gameObject.transform.Find("LowBody/LowManSkeleton/LowManHips/LowManSpine/LowManSpine1/LowManSpine2/LowManRightShoulder/LowManRightArm/LowManRightForeArm/LowManRightHand");
        if (!AIStats) AIStats = context.gameObject.GetComponent<AIStats>();
    }

    protected override void OnStop() {
        if(context.animator.GetBool("isThrowing") == true)
           context.animator.SetBool("isThrowing", false);
        //MonoBehaviour.Destroy(pivot.gameObject);
    }

    protected override State OnUpdate() {
        #region Debug line
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(context.transform.position, blackboard.AISetting.attackRange);        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(context.transform.position, blackboard.AISetting.searchRange);
        */
        #endregion
        //Need to update distance when state.running
        float distanceToPlayer = Vector3.Distance(context.agent.transform.position, blackboard.player.transform.position);
        blackboard.distanceToPlayer = distanceToPlayer;

        //ToDo : (Optional) Separate Chase and Attack function on their own script
        if (blackboard.distanceToPlayer <= AIStats.attackRange + 2f)
        {//Buffer : Prevent enemy cancelling attack when player leave a small distance from attackrange
            if (blackboard.distanceToPlayer <= AIStats.attackRange)
            {//Attack
                flag = 1;
                Attack();                
                return State.Running;
            }
            else if (flag == 1)            
            {//Keep doing the attack when Buffer still fulfilled                
                Attack();
                return State.Running;
            }
            else
            {//Nothing should run here
                return State.Running;
            }
        }
        else 
        {//Chase
            context.animator.SetBool("inAttackRange", false);
            if (pivot != null)
                MonoBehaviour.Destroy(pivot.gameObject);
            context.agent.SetDestination(blackboard.player.transform.position);
            flag = 0;
            return State.Success;
        }
        
    }

    public void Attack()
    {
        context.agent.SetDestination(context.agent.transform.position);
        context.animator.SetBool("inAttackRange", true);
        PivotInitialise();

        pivot.LookAt(blackboard.player.transform); // Make the pivot look at the target direction        
        // Reset the rotation of the parent object to maintain its original orientation
        context.transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);

        if (!alreadyAttacked)
        {            
            context.animator.SetBool("isThrowing", true);
            AnimatorStateInfo stateInfo = context.animator.GetCurrentAnimatorStateInfo(0);
            ///string currentAnimationName = stateInfo.IsName("Base Layer.Throw") ? "Throw" : "None";            
            if (stateInfo.IsName("Throw"))
            {
                //Calculate direction from attackPoint to targetPoint
                Vector3 directionWithoutSpread = blackboard.player.transform.position - attackPoint.position;

                //Calculate spread
                float x = UnityEngine.Random.Range(-AIStats.spread, AIStats.spread);
                float y = UnityEngine.Random.Range(-AIStats.spread, AIStats.spread);

                //Calculate new direction with spread
                Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

                //Instantiate bullet/projectile
                Rigidbody rb = MonoBehaviour.Instantiate(blackboard.enemyBulletObj, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();

                //Rotate bullet to shoot direction
                rb.transform.forward = directionWithSpread.normalized;

                //Add forces to bullet
                rb.AddForce(directionWithSpread.normalized * AIStats.shootingForce, ForceMode.Impulse);
                rb.AddForce(pivot.up * AIStats.shootingUpwardForce, ForceMode.Impulse);

                alreadyAttacked = true;
                StartInvoke(AIStats.attackCooldown, ResetAttack);
            }
        }
        else
        {        
            context.animator.SetBool("isThrowing", false);
        }
    }

    private void PivotInitialise()
    {
        if (pivot == null)
        {
            // Create the pivot GameObject
            pivot = new GameObject("Pivot").transform;
            pivot.SetParent(context.transform); // Set the parent as the parent object

            // Position the pivot relative to the parent object (adjust as needed)
            pivot.localPosition = Vector3.zero;

            // Set the initial local rotation of the parent object to maintain its orientation
            context.transform.localRotation = Quaternion.identity;
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        //context.animator.SetBool("isThrowing", false);
        //Debug.Log("false throw");
    }

    public void StartInvoke(float delay, Action method)
    {
        timer = new Timer(_ => method.Invoke(), null, Mathf.RoundToInt(delay * 1000), System.Threading.Timeout.Infinite);
    }
}
