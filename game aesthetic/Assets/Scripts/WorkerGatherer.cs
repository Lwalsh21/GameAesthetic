using UnityEngine;
using UnityEngine.AI;

public class WorkerGatherer : MonoBehaviour
{
    [Header("Gathering Settings")]
    public int carryCapacity = 20;
    public float gatherInterval = 1f;

    private int carryingAmount = 0;
    private ResourceManager.ResourcesType carryingType;

    private float gatherTimer = 0f;

    private ResourceNode targetNode;
    private Transform dropOffPoint;

    private NavMeshAgent agent;

    private enum State
    {
        Idle,
        MovingToNode,
        Gathering,
        ReturningToDropOff
    }

    private State currentState = State.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void AssignJob(ResourceNode node, Transform dropOff)
    {
        targetNode = node;
        dropOffPoint = dropOff;

        currentState = State.MovingToNode;
        agent.SetDestination(node.transform.position);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.MovingToNode:
                HandleMoveToNode();
                break;

            case State.Gathering:
                HandleGathering();
                break;

            case State.ReturningToDropOff:
                HandleReturnToDropOff();
                break;
        }
    }

    private void HandleMoveToNode()
    {
        if (targetNode == null)
        {
            currentState = State.Idle;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            currentState = State.Gathering;
            gatherTimer = 0f;
        }
    }

    private void HandleGathering()
    {
        if (targetNode == null)
        {
            currentState = State.Idle;
            return;
        }

        gatherTimer += Time.deltaTime;

        if (gatherTimer >= gatherInterval)
        {
            gatherTimer = 0f;

            int gathered = targetNode.Gather();

            if (gathered > 0)
            {
                carryingAmount += gathered;
                carryingType = targetNode.resourceType;
            }

            if (carryingAmount >= carryCapacity || targetNode.IsDepleted)
            {
                currentState = State.ReturningToDropOff;
                agent.SetDestination(dropOffPoint.position);
            }
        }
    }

    private void HandleReturnToDropOff()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            ResourceManager.Instance.IncreaseResource(carryingType, carryingAmount);
            carryingAmount = 0;

            if (targetNode != null && !targetNode.IsDepleted)
            {
                currentState = State.MovingToNode;
                agent.SetDestination(targetNode.transform.position);
            }
            else
            {
                currentState = State.Idle;
            }
        }
    }
}
