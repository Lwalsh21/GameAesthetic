using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    NavMeshAgent agent;
    AttackController attackController;

    public float outOfRange = 1.2f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController.SetAttackMaterial();
        agent = animator.GetComponent<NavMeshAgent>();
        attackController = animator.GetComponent<AttackController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackController.targetToAttack != null && animator.transform.GetComponent<UnitMovement>().movementCommandGiven == false)
        {
            LookAtTarget();

            agent.SetDestination(attackController.targetToAttack.position);

            var damagetoInflict = attackController.unitdamage;

            attackController.targetToAttack.GetComponent<Enemy>().ReceiveDamage(damagetoInflict);

            float distanceToTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
            if (distanceToTarget <= outOfRange || attackController.targetToAttack == null)
            {
                
                animator.SetBool("Attack", false);
            }
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = attackController.targetToAttack.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void OndrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(agent.transform.position, 10f*0.4f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(agent.transform.position, 1f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(agent.transform.position, 1.2f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
