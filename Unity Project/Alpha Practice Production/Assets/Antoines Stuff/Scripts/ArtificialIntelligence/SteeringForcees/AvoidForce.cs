using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidForce : SteeringForce
{



    public override Vector3 GetForce(AgentActor agent)
    {
        //Vector3 dirToPlayer;
        //dirToPlayer = player.transform.position - this.transform.position;
        //dirToPlayer.Normalize();

        //transform.position += dirToPlayer * player.Speed() * Time.fixedDeltaTime;

        //float dynamicLength = (agent.GetVelocity()).magnitude/agent.

        agent.gameObject.transform.LookAt(m_target.transform);
        Vector3 force = (m_target.transform.position - agent.gameObject.transform.position).normalized * agent.MaxSpeed;
        return (force - agent.GetVelocity());
    }

    public void SetTarget(GameObject target)
    {
        m_target = target;
    }

    public void Testing()
    {
    }

    private GameObject m_target;
}
