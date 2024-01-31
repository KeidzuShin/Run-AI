using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard {
        [Header("NPC Stats")]
        public float health;

        [Header("Unique Data")]
        public float distanceToPlayer;
        public float waitTimeRemaining;
        public bool playerInRange;
        public bool GoalReached;
        public Vector3 moveToPosition;

        [Header("References")]             
        public GameObject player;
        public GameObject finish;
        public GameObject spawn;
        public GameObject countdownObj;
        //public GameObject playerBulletObj;
        public GameObject enemyBulletObj;
        public TextMeshProUGUI countdown;
        public AIGameSetting AISetting;
        public AIStats AIStats;        
    }
}