using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerAgent : AgentActor {

    private FirstPersonController m_player;

	// Use this for initialization
	void Start () {
        m_player = GameObject.FindObjectOfType<FirstPersonController>();
        m_position = m_player.transform.position;
        //m_velocity;
        //m_acceleration;
        //m_force;
    }

    // Update is called once per frame
    void FixedUpdate () {
        //UpdateBehaviours();
    }
}
