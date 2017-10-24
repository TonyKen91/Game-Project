using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : AgentActor {

    //[SerializeField]
    private GameObject player;
    private FPSController playerScript;
    public SteeringBehaviour seekBehaviour;
    public SeekForce m_seekForce;

    // Use this for initialization
    void Start () {
        // Initialise behaviour list
        m_behaviours = new List<IBehaviour>();

        // Find the player game object
        player = FindObjectOfType<FPSController>().gameObject;
        playerScript = player.GetComponent<FPSController>();

        // Set up the seek force and seek force parameter
        m_seekForce = new SeekForce();
        m_seekForce.SetTarget(playerScript);
        
        // Set up the seek behaviour
        seekBehaviour = new SteeringBehaviour();
        seekBehaviour.Constructor();
        seekBehaviour.AddNewForce(m_seekForce);

        // Set up condition for chase sequence
        WithinRange chaseCondition = new WithinRange();
        chaseCondition.SetParameters(player, 20);

        // Set up chase sequence
        Sequence chaseSequence = new Sequence();
        chaseSequence.addBehaviour(chaseCondition);
        chaseSequence.addBehaviour(seekBehaviour);

        // Add 
        m_behaviours.Add(chaseSequence);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateBehaviours();

    }
}
