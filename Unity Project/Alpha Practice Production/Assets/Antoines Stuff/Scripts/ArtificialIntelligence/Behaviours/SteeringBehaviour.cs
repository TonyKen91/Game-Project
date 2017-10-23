using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour : IBehaviour {

	// Use this for initialization
	public void Constructor () {
        m_forces = new LinkedList<SteeringForce>();
    }

    public void AddNewForce(SteeringForce newForce)
    {
        // m_forces.AddLast(newForce);
        m_forces.AddLast(newForce);
    }

    // Update is called once per frame
    public BehaviourResult UpdateBehaviour(AgentActor agent)
    {
        foreach(SteeringForce force in m_forces)
            agent.AddForce(force.GetForce(agent));
        return BehaviourResult.SUCCESS;
    }

    private LinkedList<SteeringForce> m_forces;
}
