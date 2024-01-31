using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public AudioClip footstepSound;
    public AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void FootstepSound()
    {
        if (footstepSound != null)
        {
            // Play the footstep sound
            audioSource.PlayOneShot(footstepSound);
        }
    }
}
