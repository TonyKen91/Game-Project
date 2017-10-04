using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SeekForce : ISteeringForce {


    public Vector3 GetForce(AgentActor agent)
    {
        //Vector3 dirToPlayer;
        //dirToPlayer = player.transform.position - this.transform.position;
        //dirToPlayer.Normalize();

        //transform.position += dirToPlayer * player.Speed() * Time.fixedDeltaTime;



        Vector3 force = (m_target.transform.position - agent.transform.position).normalized * agent.MaxSpeed;
        return (force - agent.GetVelocity());
    }

    public void SetTarget(AgentActor target)
    {
        m_target = target;
    }

    private AgentActor m_target;
}
