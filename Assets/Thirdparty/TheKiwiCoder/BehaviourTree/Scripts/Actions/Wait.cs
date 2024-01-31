using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TheKiwiCoder {

    [System.Serializable]
    public class Wait : ActionNode {

        public float timerInSeconds = 1;
        float remainingTime;

        protected override void OnStart() {
            remainingTime = timerInSeconds;            
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            remainingTime -= Time.deltaTime; // Decrease the remaining time by the elapsed time since the last frame
            blackboard.waitTimeRemaining = remainingTime; // Save number to blackboard
            if (remainingTime < 0f)
            {
                blackboard.waitTimeRemaining = 0;
                return State.Success;
            }
            return State.Running;
        }
    }
}
