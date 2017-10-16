using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour : IBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void AddNewForce(ISteeringForce newForce)
    {
        m_forces.AddLast(newForce);
    }

    // Update is called once per frame
    public void UpdateBehaviour(AgentActor agent)
    {
        foreach(ISteeringForce force in m_forces)
            agent.AddForce(force.GetForce(agent));
    }

    private LinkedList<ISteeringForce> m_forces;
}
