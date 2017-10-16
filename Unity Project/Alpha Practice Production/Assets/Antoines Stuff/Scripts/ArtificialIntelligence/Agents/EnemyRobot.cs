using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : AgentActor {

    //[SerializeField]
    private GameObject player;
    private PlayerAgent playerScript;

	// Use this for initialization
	void Start () {

        player = FindObjectOfType<PlayerAgent>().gameObject;
        playerScript = player.GetComponent<PlayerAgent>();
        SeekForce m_seekForce = new SeekForce();
        m_seekForce.SetTarget(playerScript);

        m_behaviours.Add(new SteeringBehaviour());

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateBehaviours();
	}
}
