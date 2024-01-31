using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookAtPlayer : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(player.activeInHierarchy) transform.LookAt(player.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180f, 0);
    }
}
