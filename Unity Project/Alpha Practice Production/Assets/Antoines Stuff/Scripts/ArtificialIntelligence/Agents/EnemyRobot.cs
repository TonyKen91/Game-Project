using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : AgentActor {

    //[SerializeField]
    private GameObject player;
    private FPSController playerScript;
    private SteeringBehaviour seekBehaviour;
    private SeekForce m_seekForce;

    // Use this for initialization
    void Start () {
        // Initialise behaviour list
        m_behaviours = new List<IBehaviour>();

        // Find the player game object
        player = FindObjectOfType<FPSController>().gameObject;
        playerScript = player.GetComponent<FPSController>();


        //-----------------------------------------------------------------
        // The Collision Avoidance Sequence

        // Set up the collsion avoidance force



        ////-----------------------------------------------------------------
        //// The Attack Sequence

        //// Set up the attack behaviour
        //ShootBehaviour m_attackBehaviour = new ShootBehaviour();


        //// Set up condition for the attack sequence
        //WithinRange attackCondition = new WithinRange();
        //attackCondition.SetParameters(player, 5);

        //// Set up attack sequence
        //Sequence attackSequence = new Sequence();
        //attackSequence.addBehaviour(attackCondition);
        //attackSequence.addBehaviour(m_attackBehaviour);
        
        
        
//-----------------------------------------------------------------
// The Chase Sequence

        // Set up the seek force and seek force parameter
        m_seekForce = new SeekForce();
        m_seekForce.SetTarget(player);
        
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



//----------------------------------------------------------------
// The Patrol Sequence

        // Set up patrol behaviour
        PatrolBehaviour patrol = new PatrolBehaviour();
        patrol.StartUp();
        patrol.SetPatrolPoints(new Vector3(20, 3, 12), new Vector3(-20, 3, 12));



//----------------------------------------------------------------
// The Main Selector

        // Set up main selector
        Selector mainSelector = new Selector();
        mainSelector.addBehaviour(chaseSequence);
        mainSelector.addBehaviour(patrol);



        // Add all sequences to behaviour list
        m_behaviours.Add(mainSelector);

        // Setting the forward direction
        transform.forward = new Vector3(0, 0, 1);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateBehaviours();
        //transform.LookAt(player.transform);
    }
}
