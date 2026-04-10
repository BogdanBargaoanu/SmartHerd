using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform target;

    private Vector3 lastTargetPosition;

    public float recalculateThreshold = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (target != null)
        {
            lastTargetPosition = target.position;
            agent.SetDestination(target.position);
        }
    }

    void Update()
    {
        if (target != null)
        {
            if (Vector3.Distance(target.position, lastTargetPosition) > recalculateThreshold)
            {
                agent.SetDestination(target.position);
                lastTargetPosition = target.position;
            }

            if (!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                Debug.LogWarning("The destination is blocked or inside an obstacle. Stopping as close as possible.");
            }
        }
    }
}
