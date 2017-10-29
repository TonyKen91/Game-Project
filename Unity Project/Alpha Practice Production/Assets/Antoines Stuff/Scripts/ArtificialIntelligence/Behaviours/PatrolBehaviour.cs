using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : IBehaviour
{
    private Vector3 patrolPointA;
    private Vector3 patrolPointB;
    private Vector3 currentTarget;
    private Transform pointTransform;

    public void SetPatrolPoints(Vector3 pointA, Vector3 pointB)
    {
        patrolPointA = pointA;
        patrolPointB = pointB;
    }

    public void StartUp()
    {
        // m_forces.AddLast(newForce);
        currentTarget = patrolPointA;
    }

    // Update is called once per frame
    public BehaviourResult UpdateBehaviour(AgentActor agent)
    {
        Vector3 agentPos = agent.gameObject.transform.position;


        if ((agentPos - currentTarget).sqrMagnitude <= 10)
        {
            if (currentTarget == patrolPointA)
                currentTarget = patrolPointB;
            else
                currentTarget = patrolPointA;
        }
        agent.gameObject.transform.LookAt(currentTarget);

        Vector3 force = (currentTarget - agentPos).normalized * agent.MaxSpeed;
        agent.AddForce(force - agent.GetVelocity());

        return BehaviourResult.SUCCESS;
    }

    //private LinkedList<SteeringForce> m_forces = new LinkedList<SteeringForce>();
}
