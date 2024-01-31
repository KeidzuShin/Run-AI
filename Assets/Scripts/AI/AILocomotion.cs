using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    GameSetting GameSetting;
    Rigidbody[] rigidbodies;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        GameSetting = Resources.Load<GameSetting>("GameSetting");
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagdoll();
    }

    void Update()
    {
        #region Walk / Sprint Animation
        if (agent.velocity.x != 0)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("Vertical", Mathf.MoveTowards(animator.GetFloat("Vertical"), 1f, GameSetting.WalkDelta * Time.deltaTime));
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("Vertical", Mathf.MoveTowards(animator.GetFloat("Vertical"), 0f, GameSetting.WalkDelta * Time.deltaTime));
        }
        #endregion

        /*
        #region Jump / Fall Animation
        if (agent.velocity.y > 1)
        {
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
        else
        {
            animator.SetBool("isGrounded", false);
            if (agent.velocity.y != 0)
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
            }
        }
        #endregion
        */
    }

    public void DeactivateRagdoll()
    {
        foreach(var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }   
        animator.enabled= true;
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        animator.enabled = false;
    }
}
