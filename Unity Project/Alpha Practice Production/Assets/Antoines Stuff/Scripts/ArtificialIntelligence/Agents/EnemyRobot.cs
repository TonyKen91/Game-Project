using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : AgentActor {

    //[SerializeField]
    private GameObject player;
    private FPSController playerScript;
    public SteeringBehaviour steering;
    public SeekForce m_seekForce;

    // Use this for initialization
    void Start () {
        m_behaviours = new List<IBehaviour>();
        player = FindObjectOfType<FPSController>().gameObject;
        playerScript = player.GetComponent<FPSController>();
        m_seekForce = new SeekForce();
        m_seekForce.SetTarget(playerScript);
        steering = new SteeringBehaviour();
        steering.Constructor();
        steering.AddNewForce(m_seekForce);
        m_behaviours.Add(steering);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateBehaviours();

    }
}
