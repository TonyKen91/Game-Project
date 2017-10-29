using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{

    //public variables
    public Slider m_healthSlider;
    public Slider m_staminaSlider;
    public Slider m_hungerSlider;
    public Slider m_thirstSlider;


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
    [SerializeField] private float m_boostMax = 3;
    [SerializeField] private float m_boostCoolDown = 0;
    [SerializeField] private float m_boostCoolDownMax = 1;
    [SerializeField] private float m_boostAmount;


    //Looking/camera variables
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;

    //health/stamina variables
    [SerializeField] private int m_maxHealth = 100;
    [SerializeField] private float m_maxStamina = 5;
    [SerializeField] private float m_maxHunger = 100;
    [SerializeField] private float m_maxThirst = 100;
    [SerializeField] private float m_statDepletionRate = 15;

    //private variables

    //health/stamina variables
    private int m_curHealth;
    private float m_curStamina;
    private float m_curHunger;
    private float m_curThirst;
    private float m_statTimer;


    //walking/sprinting variables
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private bool m_canSprint = true;
    private float m_sprintCooldown = 1.0f;
    private bool m_crouching;

    //jumping/falling variables
    private bool m_canJump;
    private bool m_PreviouslyGrounded;
    private bool m_Airborne;
    private float m_StickToGroundForce = 20.0f;
    private float m_GravityMultiplier = 2.0f;
    private bool m_boosting = false;


    //looking/camera variables
    private Camera m_Camera;
    private float m_YRotation;

    //collision variables
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;




    // Use this for initialization
    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_Airborne = false;
        m_MouseLook.Init(transform, m_Camera.transform);
        m_curHealth = m_maxHealth;
        m_curStamina = m_maxStamina;
        m_curHunger = m_maxHunger;
        m_curThirst = m_maxThirst;
        m_statTimer = 0.0f;
        m_boostAmount = m_boostMax;
        m_boostCoolDown = m_boostCoolDownMax;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatHandler();
        RotateView();
        JumpHandling();
        CrouchHandling();
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
            }

            if (m_boostAmount <= m_boostMax && m_boostAmount >= 0)
            {
                m_boostAmount += Time.fixedDeltaTime * 0.5f;
            }
        }
        //the player is not on the ground
        else
        {
            //allow for control in the air
            if (m_airControl)
            {
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;
            }

            //apply gravity
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            
            //if boosting and boost amount is greater than 0 and the boost cool down is greater than or equal to the boost cool down maximum
            if (m_boosting && m_boostAmount > 0 && (m_boostCoolDown >= m_boostCoolDownMax))
            {
                m_boostAmount -= Time.fixedDeltaTime;
                if (m_boostAmount < 0)
                {
                    m_boostAmount = 0.0f;
                    m_boostCoolDown = 0;
                }
                if (m_MoveDir.y <= 0)
                {
                    m_MoveDir.y += -m_MoveDir.y * 0.1f;
                }
                m_MoveDir.y += m_BoostSpeed;
            }
            else if (m_boostAmount < m_boostMax && m_boostAmount >= 0)
            {
                m_boostAmount += Time.fixedDeltaTime * 0.5f;

                //if not boosting
                if (!m_boosting)
                {

                    //if boost cool down is less than boost max
                    if (m_boostCoolDown < m_boostCoolDownMax)
                    {
                        m_boostCoolDown += Time.fixedDeltaTime;
                        //truncate the boost cool down
                        if (m_boostCoolDown >= m_boostCoolDownMax)
                        {
                            m_boostCoolDown = m_boostCoolDownMax;
                        }
                    }
                }
            }
            m_boosting = false;
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

        if (!m_Airborne)
        {
            //if player is sprinting, decrease stamina
            if (!m_walking)
            {
                m_curStamina -= Time.fixedDeltaTime * 2;
                m_sprintCooldown = 2;
            }

            //otherwise replenish stamina and sprint cooldown
            if (m_curStamina < m_maxStamina)
            {
                m_curStamina += Time.fixedDeltaTime / 1.5f;
                m_sprintCooldown -= Time.deltaTime;
            }
        }
        

        float percentage = (m_curStamina / m_maxStamina) * 100;
        m_staminaSlider.value = percentage;

    }

    private void JumpHandling()
    {
        //input if player wants to jump
        if (!m_canJump && !m_Airborne)
        {
            m_canJump = Input.GetButtonDown("Jump");
        }
        if (m_Airborne && m_boostCoolDown >= m_boostCoolDownMax)
        {
            m_boosting = Input.GetButton("Jump");
        }

        //if player was not on the ground last frame and is currently grounded
        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {
            float yVel = m_MoveDir.y;
            if (yVel <= -m_maxFallVel)
            {
                if (yVel > -25)
                {
                    ApplyDamage((int)(yVel * -0.7f));
                }
                else if (yVel <= -25 && yVel > -30)
                {
                    ApplyDamage((int)(yVel * -0.95f));
                }
                else if (yVel <= -30 && yVel > -40)
                {
                    ApplyDamage((int)(yVel * -1.5f));
                }
                else if (yVel <= -40)
                {
                    ApplyDamage((int)(yVel * -3f));
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

    private void PlayerStatHandler()
    {
        m_statTimer += Time.deltaTime;

        if (m_statTimer >= m_statDepletionRate)
        {
            if (m_curHunger > 0)
            {
                m_curHunger -= 1;
                m_hungerSlider.value = m_curHunger;
            }
            if (m_curThirst > 0)
            {
                m_curThirst -= 1;
                m_thirstSlider.value = m_curThirst;
            }
            m_statTimer = 0;
           
        }
    }

    private void CrouchHandling()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && m_crouching == false)
        {
            m_crouching = true;
            Vector3 tempScale = this.transform.localScale;
            tempScale.y *= 0.5f;
            this.transform.localScale = tempScale;

        }

        if (m_crouching)
        {
            Ray crouchRay = new Ray(transform.position, transform.up);
            RaycastHit info;
            Physics.Raycast(crouchRay, out info);
            if (info.collider == null)
            {

                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    m_crouching = false;
                    Vector3 tempScale = this.transform.localScale;
                    tempScale.y *= 2f;
                    this.transform.localScale = tempScale;
                }
            }
           
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

        if (m_CollisionFlags == CollisionFlags.Above)
        {
            m_MoveDir.y = 0;
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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //if player can sprint, check if shift is pressed, else set player to be walking
        if (m_canSprint)
        {
            m_walking = !Input.GetKey(KeyCode.LeftShift);
        }
        else
        {
            m_walking = true;
        }

        // set the desired speed to be walking or running
        if (!m_Airborne)
        {
            speed = m_walking ? m_WalkSpeed : m_RunSpeed;
        }
        else //set the speed while player is jumping to walkspeed
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
        if (m_curHealth < 0)
        {
            m_curHealth = 0;
        }
        m_healthSlider.value = m_curHealth;
    }

}
