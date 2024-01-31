using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RegenHealth : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
        blackboard.AIStats.currentHealth = blackboard.AIStats.lowHealth + 2f;
    }

    protected override State OnUpdate() {
        blackboard.health = blackboard.AIStats.currentHealth;
        blackboard.AIStats.currentHealth = Mathf.MoveTowards(
            blackboard.AIStats.currentHealth, 
            blackboard.AIStats.maxHealth, 
            blackboard.AIStats.regenRate * Time.deltaTime
            );
        if(blackboard.AIStats.currentHealth <= blackboard.AIStats.lowHealth + 1f)
            return State.Running;
        else
            return State.Success;
    }
}
