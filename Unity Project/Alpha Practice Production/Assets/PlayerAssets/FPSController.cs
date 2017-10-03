using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour {

    //variables
    [SerializeField] private bool m_walking = true;
    [SerializeField] private float m_WalkSpeed = 5.0f;
    [SerializeField] private float m_RunSpeed = 11.0f;
    [SerializeField] private float m_JumpSpeed = 10.0f;
    [SerializeField] private float m_StickToGroundForce = 20.0f;
    [SerializeField] private float m_GravityMultiplier = 2.0f;
    [SerializeField] private bool m_airControl = false;
    [SerializeField] private float m_fallDamageDistance = 2.0f;
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private float m_maxStamina = 5;
    [SerializeField] private int m_curHealth;
    [SerializeField] private float m_curStamina;

    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;
    private float m_fallDistanceStart;
    private bool m_canSprint = true;
    private float m_sprintCooldown = 1.0f;



    // Use this for initialization
    private void Start () {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_Jumping = false;
        m_MouseLook.Init(transform, m_Camera.transform);
        m_curHealth = m_maxHealth;
        m_curStamina = m_maxStamina;
    }
	
	// Update is called once per frame
	void Update () {

        RotateView();
        // the jump state needs to read here to make sure it is not missed
        
        //input if player wants to jump
        if (!m_Jump)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        //if player was not on the ground last frame and is currently grounded
        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            m_MoveDir.y = 0f;
            m_Jumping = false;
            //do the fall damage here i think?
            float distance = m_fallDistanceStart - m_fallDamageDistance;
            if (this.transform.position.y <= distance)
            {
                m_curHealth -= (int)(distance * 3);
            }
        }
        //if player is not grounded, not jumping but was grounded last frame
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
            m_Jumping = true;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    //fixed update, otherwise known as physics update
    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);
        SprintHandling();
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                           m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;


        //if the player is on the ground
        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;
            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;
            //if player has started to jump
            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                m_fallDistanceStart = this.transform.position.y;
                m_Jump = false;
                m_Jumping = true;
            }
        }
        //the player is not on the ground
        else
        {
            if (m_Jump)
            {
              m_Jump = false;
              m_Jumping = true;
              m_fallDistanceStart = this.transform.position.y;

            }

            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            if (m_airControl)
            {
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;
            }
        }
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

        m_MouseLook.UpdateCursorLock();
    }

    //FIX THIS, SPRINT WILL NEVER ACTUALLY STOP
    private void SprintHandling()
    {
        if (m_curStamina <= 0)
        {
            m_canSprint = false;
            m_walking = true;
        }
        else if (m_sprintCooldown <= 0)
        {
            m_canSprint = true;
        }
        if (!m_walking)
        {
            m_curStamina -= Time.fixedDeltaTime;
            m_sprintCooldown = 2;
        }
        else if (m_curStamina < m_maxStamina)
        {
            m_curStamina += Time.fixedDeltaTime / 1.5f;
            m_sprintCooldown -= Time.deltaTime;

        }

    }

    //rotate the player
    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = m_walking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        /*
        if (m_canSprint == true && Input.GetKey(KeyCode.LeftShift))
        {
            m_walking = false;
        } else
        {
            m_walking = true;
        }
        */

        if (m_canSprint)
        {
            m_walking = !Input.GetKey(KeyCode.LeftShift);
        } else
        {
            m_walking = true;
        }
       
#endif
        // set the desired speed to be walking or running
        if (!m_Jumping)
        {
            speed = m_walking ? m_WalkSpeed : m_RunSpeed;
        } else
        {
            speed = m_WalkSpeed;
        }
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_walking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_walking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }

    public Vector3 OutputPlayerMovement()
    {

        return m_MoveDir;
    }
}
