using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using TMPro;


[System.Serializable]
public class GetBlackboardAndRepeat : DecoratorNode
{
    [Header("References")]
    public GameObject player;
    public GameObject spawn;
    public GameObject finish;
    public GameObject countdownObj;
    //public GameObject playerBulletObj;
    public GameObject enemyBulletObj;
    public TextMeshProUGUI countdown;
    public AIGameSetting AISetting;
    public AIStats AIStats;

    public bool restartOnSuccess = true;
    public bool restartOnFailure = false;
    public void SetCountDown(GameObject Object)
    {
        countdown = Object.GetComponent<TextMeshProUGUI>();
    }
    protected override void OnStart() {
        #region Find References
        player = GameObject.FindGameObjectWithTag("Player");
        finish = GameObject.FindGameObjectWithTag("Finish");
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        countdownObj = GameObject.FindGameObjectWithTag("Countdown");
        AISetting = Resources.Load<AIGameSetting>("AIGameSetting");
        AIStats = context.gameObject.GetComponent<AIStats>();
        //playerBulletObj = Resources.Load<GameObject>("PlayerBulletObj");
        enemyBulletObj = Resources.Load<GameObject>("EnemyBulletObj");
        if(countdownObj) SetCountDown(countdownObj);
        #endregion

        #region Write to blackboard
        blackboard.player = player;
        blackboard.spawn = spawn;
        blackboard.finish = finish;
        blackboard.countdownObj = countdownObj;
        blackboard.countdown = countdown;
        //blackboard.playerBulletObj = playerBulletObj;
        blackboard.enemyBulletObj = enemyBulletObj;
        blackboard.AISetting = AISetting;
        blackboard.AIStats = AIStats;
        #endregion

        #region Initializer (AIStats) 
        //ToDo : randomize the value for each NPC
        blackboard.health = AIStats.currentHealth;
        #endregion
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        #region Repeat
        switch (child.Update())
        {
            case State.Running:
                break;
            case State.Failure:
                if (restartOnFailure)
                    return State.Running;
                else
                    return State.Failure;
            case State.Success:
                if (restartOnSuccess)
                    return State.Running;
                else
                    return State.Success;
        }
        #endregion
        return State.Running;
    }
}
