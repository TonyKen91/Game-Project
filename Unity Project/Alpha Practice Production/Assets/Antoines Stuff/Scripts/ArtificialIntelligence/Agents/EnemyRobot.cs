using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : AgentActor {

	// Use this for initialization
	void Start () {
        //SeekForce m_seekForce = new SeekForce;
        //m_seekForce.SetTarget(player)
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateBehaviours();
	}
}
