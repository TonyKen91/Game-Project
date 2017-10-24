using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SeekForce : SteeringForce {

    

    public override Vector3 GetForce(AgentActor agent)
    {
        //Vector3 dirToPlayer;
        //dirToPlayer = player.transform.position - this.transform.position;
        //dirToPlayer.Normalize();

        //transform.position += dirToPlayer * player.Speed() * Time.fixedDeltaTime;

        Transform targetTransform = m_target.gameObject.transform;


        Vector3 force = (targetTransform.position - agent.gameObject.transform.position).normalized * agent.MaxSpeed;
        return (force - agent.GetVelocity());
    }

    public void SetTarget(FPSController target)
    {
        m_target = target;
    }

    public void Testing()
    {
    }

    private FPSController m_target;
}
