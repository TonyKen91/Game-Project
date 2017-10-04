using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyActor : MonoBehaviour {


    private CharacterController controller;

    private Rigidbody rb;

    public float speed = 5.0f;
    //public float jumpForce = 100.0f;

    //private Vector3 m_moveDirection;

    private Rigidbody enemyBody;

    public Transform target;

    //private Vector3 agentTransformation;

    // Use this for initialization
    void Start () {
        enemyBody = gameObject.GetComponent<Rigidbody>();
        //agentTransformation = this.transform.position - target.position;
    }

    // Update is called once per frame
    //void Update () {
    //    Vector3 move_direction = new Vector3(0, 0, 0);
    //    Vector3 target_pos = target.position + agentTransformation;
    //    this.transform.position = target_pos;

    //    move_direction.Set(speed * target_pos.x, speed * target_pos.y, speed * target_pos.z);
    //    controller.Move(move_direction * Time.deltaTime);

    //    //m_moveDirection.x = Input.GetAxis("Horizontal") * speed;

    //    //if (m_character.isGrounded)
    //    //{
    //    //    m_moveDirection.y = 0;
    //    //}

    //    //else
    //    //{
    //    //    m_moveDirection.y += Physics.gravity * Time.fixedDeltaTime;
    //    //}

    //    //if (Input.GetButtonDown("Jump"))
    //    //{
    //    //    m_moveDirection.y = jumpForce;
    //    //}
    //    //else
    //    //    m_moveDirection.y = 0;

    //    //m_character.Move(m_moveDirection * Time.fixedDeltaTime);
    //    // Acceleration and Gyro for mobile?
    //    // custom input
    //    // assets
    //    // cross platform, control, unity system
    //}

    void FixedUpdate()
    {

        Vector3 moveToTarget = (target.position - enemyBody.position);
        moveToTarget.Normalize();
        moveToTarget = (moveToTarget * speed) - enemyBody.velocity;
        enemyBody.position += moveToTarget * Time.fixedDeltaTime;
    }

}
