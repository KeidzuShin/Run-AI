using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;
using System.Threading;

[System.Serializable]
public class isLowHealth : ActionNode
{
    private Timer timer;
    public float health;
    protected override void OnStart() {        
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        blackboard.health = blackboard.AIStats.currentHealth;
        if(blackboard.AIStats.currentHealth <= blackboard.AIStats.lowHealth)
            return State.Success;
        else
            return State.Failure;
    }
}
