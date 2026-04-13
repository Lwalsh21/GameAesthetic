using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class UnitFollowState : StateMachineBehaviour
{
    AttackController attackController;

    NavMeshAgent agent;
    public float attackRange = 1f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.transform.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
                if (attackController.targetToAttack == null)
        {
            animator.SetBool("Follow", false);
        }
        else
        {
            if (animator.transform.GetComponent<UnitMovement>().movementCommandGiven == false)
            {
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);

                float distanceToTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
                if (distanceToTarget <= attackRange)
                {
                    agent.SetDestination(animator.transform.position);
                    animator.SetBool("Attack", true);
                }
            }
        }

        
    }
}   