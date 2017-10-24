using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour {

    //public variables
    public Slider m_healthSlider;
    public Slider m_staminaSlider;


    //Serialized Fields, private variables that can be edited in the inspector
    //walking/sprinting variables
    [SerializeField] private bool m_walking = true;
    [SerializeField] private float m_WalkSpeed = 5.0f;
    [SerializeField] private float m_RunSpeed = 11.0f;
    [SerializeField] private float m_JumpSpeed = 10.0f;
    [SerializeField] private float m_BoostSpeed = 20.0f;

    //jumping/falling variables
    [SerializeField] private bool m_airControl = false;
    [SerializeField] private float m_maxFallVel;

    //Looking/camera variables
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();

    //health/stamina variables
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private float m_maxStamina = 5;
    [SerializeField] private int m_curHealth;
    [SerializeField] private float m_curStamina;


    //private variables

    //walking/sprinting variables
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private bool m_canSprint = true;
    private float m_sprintCooldown = 1.0f;

    //jumping/falling variables
    private bool m_canJump;
    private bool m_PreviouslyGrounded;
    private bool m_Airborne;
    private float m_StickToGroundForce = 20.0f;
    private float m_GravityMultiplier = 2.0f;
    private bool m_canBoost = true;
    private bool m_boosting = false;


    //looking/camera variables
    private Camera m_Camera;
    private float m_YRotation;

    //collision variables
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;




    // Use this for initialization
    private void Start () {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_FovKick.Setup(m_Camera);
        m_Airborne = false;
        m_MouseLook.Init(transform, m_Camera.transform);
        m_curHealth = m_maxHealth;
        m_curStamina = m_maxStamina;

    }
	
	// Update is called once per frame
	void Update () {
        
        RotateView();
        JumpHandling();
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
            if (m_canJump)
            {
                m_MoveDir.y = m_JumpSpeed;
                m_canJump = false;
                m_Airborne = true;
                m_canBoost = true;
            }
        }
        //the player is not on the ground
        else
        {

            //apply gravity
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;

            //allow for control in the air
            if (m_airControl)
            {
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;
            }

            if (m_boosting)
            {
                m_boosting = false;
                m_canBoost = false;
                m_MoveDir.y = m_BoostSpeed;
            }
        }
               
        m_PreviouslyGrounded = m_CharacterController.isGrounded;
        m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
        m_MouseLook.UpdateCursorLock();
    }

    //Function to control the sprinting for the player
    private void SprintHandling()
    {
        //if stamina is gone, make player start walking and disallow sprinting
        if (m_curStamina <= 0)
        {
            m_canSprint = false;
            m_walking = true;
        }
        //if the sprint cooldown is finished, allow sprinting
        else if (m_sprintCooldown <= 0)
        {
            m_canSprint = true;
        }

        //if player is sprinting, decrease stamina
        if (!m_walking)
        {
            m_curStamina -= Time.fixedDeltaTime;
            m_sprintCooldown = 2;
        }
        //otherwise replenish stamina and sprint cooldown
        else if (m_curStamina < m_maxStamina)
        {
            m_curStamina += Time.fixedDeltaTime / 1.5f;
            m_sprintCooldown -= Time.deltaTime;
        }

        float percentage = (m_curStamina / m_maxStamina) * 100;
        m_staminaSlider.value = percentage;

    }

    private void JumpHandling()
    {
        //input if player wants to jump
        if (!m_canJump && !m_Airborne)
        {
            m_canJump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
        if (m_Airborne && m_canBoost)
        {
            m_boosting = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        //if player was not on the ground last frame and is currently grounded
        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            float yVel = m_MoveDir.y;
            if (yVel <= -m_maxFallVel)
            {
                Debug.Log(yVel);
                if (yVel > -25)
                {
                    ApplyDamage((int)(yVel * -0.7f));
                    Debug.Log("0.8");
                }
                else if (yVel <= -25 && yVel > -30)
                {
                    ApplyDamage((int)(yVel * -0.95f));
                    Debug.Log("0.95");
                }
                else if (yVel <= -30 && yVel > -40)
                {
                    ApplyDamage((int)(yVel * -1.5f));
                    Debug.Log("1.5");
                }
                else if (yVel <= -40)
                {
                    ApplyDamage((int)(yVel * -3f));
                    Debug.Log("3");
                }

            }

            m_MoveDir.y = 0f;
            m_Airborne = false;
        }

        //if player is not grounded, not jumping but was grounded last frame
        if (!m_CharacterController.isGrounded && !m_Airborne && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
            m_Airborne = true;
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

        //if player can sprint, check if shift is pressed, else set player to be walking
        if (m_canSprint)
        {
            m_walking = !Input.GetKey(KeyCode.LeftShift);
        } else
        {
            m_walking = true;
        }

        // set the desired speed to be walking or running
        if (!m_Airborne)
        {
            speed = m_walking ? m_WalkSpeed : m_RunSpeed;
        } else //set the speed while player is jumping to walkspeed
        {
            speed = m_WalkSpeed;
        }
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }
    }

    public Vector3 OutputPlayerMovement()
    {

        return m_MoveDir;
    }

    public void ApplyDamage(int dmg)
    {

        this.m_curHealth -= dmg;
        if (m_curHealth <0)
        {
            m_curHealth = 0;
        }
        m_healthSlider.value = m_curHealth;
    }
}
