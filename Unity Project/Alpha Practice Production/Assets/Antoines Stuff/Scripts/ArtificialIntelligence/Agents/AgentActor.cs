using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class AgentActor : MonoBehaviour {

    // When we want to make a prefab, we can't reference any specific object in a scene
    // This is because it could be used in multiple scenes
    //private PlayerActor player;
    // FirstPersonController player;

    protected List<IBehaviour> m_behaviours;

    protected Vector3 m_position;
    protected Vector3 m_velocity;
    protected Vector3 m_acceleration;
    protected Vector3 m_force;

    public float MaxSpeed = 10.0f;

    // Use this for initialization
    void Start()
    {
        // To overcome specifying a reference, this is used instead to find the object
        //player = GameObject.FindObjectOfType<PlayerActor>();
        //player = GameObject.FindObjectOfType<FirstPersonController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //UpdateBehaviours();
            
    }
    protected void UpdateBehaviours()
    { 
        m_force = Vector3.zero;
        foreach (IBehaviour behaviour in m_behaviours)
        {
            behaviour.UpdateBehaviour(this);
        }

        m_velocity += m_force * Time.fixedDeltaTime;
        //transform.LookAt(m_velocity);
        transform.position += m_velocity * Time.fixedDeltaTime;
    }

    public void AddForce(Vector3 force)
    {
        m_force += force;
    }

    public Vector3 GetVelocity()
    {
        return m_velocity;
    }
}
