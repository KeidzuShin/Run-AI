using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : MonoBehaviour
{
    Cinemachine.CinemachineVirtualCameraBase VirtualCam;
    public GameObject player;
    void Start()
    {
        //Find Virtual Cinemachine
        VirtualCam = FindAnyObjectByType<Cinemachine.CinemachineVirtualCameraBase>();

        //Find Player Tagged Object
        player = GameObject.FindGameObjectWithTag("Player");

        //Set Cinemachine Follow to Player Transform Position
        VirtualCam.Follow = player.transform;
    }

}
