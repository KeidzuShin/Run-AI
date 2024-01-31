using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class WaitCountdownTimer : DecoratorNode
{
    public string endMessage;
    protected override void OnStart() {
        blackboard.countdownObj.SetActive(true);
    }

    protected override void OnStop() {
        blackboard.countdown.text = endMessage;
        //blackboard.countdownObj.SetActive(false);
    }

    protected override State OnUpdate() {
        switch (child.Update())
        {
            case State.Running:
                UpdateText();
                return State.Running;
            case State.Failure:
                return State.Failure;
            case State.Success:
                return State.Success;
        }
        return State.Running;
    }
    void UpdateText()
    {
        int minutes = Mathf.FloorToInt(blackboard.waitTimeRemaining / 60f); // Calculate the number of minutes in the remaining time
        int seconds = Mathf.FloorToInt(blackboard.waitTimeRemaining % 60f); // Calculate the number of seconds in the remaining time
        int milliseconds = Mathf.FloorToInt((blackboard.waitTimeRemaining * 1000f) % 1000f); // Calculate the number of milliseconds in the remaining time
        string text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds); // Format the remaining time as "Minutes:Seconds:Milliseconds"
        blackboard.countdown.text = text; // Update the TextMeshPro component with the remaining time
    }
}
