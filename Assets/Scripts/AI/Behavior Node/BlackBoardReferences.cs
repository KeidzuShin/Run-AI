using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using TMPro;

[System.Serializable]
public class BlackBoardReferences : ActionNode
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
    public void SetCountDown(GameObject Object)
    {
        countdown = Object.GetComponent<TextMeshProUGUI>();
    }
    protected override void OnStart()
    {
        #region Find References
        player = GameObject.FindGameObjectWithTag("Player");
        finish = GameObject.FindGameObjectWithTag("Finish");
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        countdownObj = GameObject.FindGameObjectWithTag("Countdown");
        AISetting = Resources.Load<AIGameSetting>("AIGameSetting");
        AIStats = context.gameObject.GetComponent<AIStats>();
        //playerBulletObj = Resources.Load<GameObject>("PlayerBulletObj");
        enemyBulletObj = Resources.Load<GameObject>("EnemyBulletObj");
        SetCountDown(countdownObj);
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
    }

    protected override void OnStop(){
    }

    protected override State OnUpdate() {       
        return State.Success;
    }
}
