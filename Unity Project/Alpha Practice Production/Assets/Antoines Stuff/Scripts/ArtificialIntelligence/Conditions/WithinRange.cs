using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithinRange : IBehaviour {

    private GameObject m_target;
    private float m_range;

    // Use this for initialization
    void Start () {
		
	}

    public BehaviourResult UpdateBehaviour(AgentActor agent)
    {

        if ((agent.gameObject.transform.position - m_target.transform.position).sqrMagnitude <= m_range*m_range)
            return BehaviourResult.SUCCESS;
        return BehaviourResult.FAILURE;
    }

    public void SetParameters(GameObject target/*Vector3 targetPosition*/, float range)
    {
        //m_targetPosition = targetPosition;
        m_target = target;
        m_range = range;
    }
}
