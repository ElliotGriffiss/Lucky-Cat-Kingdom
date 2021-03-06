using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using DataClasses;

public class PlayerController : MonoBehaviour
{
    // Config
    [Header("Player Settings")]
    [SerializeField] private float m_speed = 10f;
    [SerializeField] private float m_jumpForce = 10f;
    [SerializeField] private float m_ExtraJumpTime = 1f;
    [Space]
    [SerializeField] private LayerMask m_groundLayerMask;
    [SerializeField] private LayerMask m_wallsLayerMask;
    [Space]
    [SerializeField] private bool IsMenuCat;

    [Header("Jump Settings")]
    [SerializeField] private float m_hangTime = 0.2f;
    [SerializeField] private float m_jumpBuffer = 0.2f;
    [SerializeField] private float m_jumpCooldown = 0.2f;
    [Space]
    [SerializeField] private float m_fallMultiplier = 2.5f;
    [SerializeField] private float m_lowJumpMultiplier = 2.5f;
    [SerializeField] private float m_highJumpMultiplier = 2.5f;
    [Space]
    [SerializeField] private float m_groundDetectionPadding = 0.1f;
    [SerializeField] private float m_wallDetectionPadding = 0.05f;

    [Header("Idle Settings")]
    [SerializeField] private float IdleTimer = 5f;

    // Death
    [Header("Death Settings")]
    [SerializeField] private float MinYValue;
    [SerializeField] private float ForceModifier = 1f;
    [SerializeField] private float DamageUplift;
    [SerializeField] private float DamageTime = 0.5f;
    private IEnumerator coroutine;
    private bool PlayerHasControl = true;
    private bool InMenus = false;

    [Header("Scene References")]
    [SerializeField] private SpriteRenderer myRenderer;
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private BoxCollider2D myCollider;
    [Space]
    [SerializeField] private TimeManager Timer;
    [Space]
    [SerializeField] private ParticleSystem LandingParticles;
    [SerializeField] private ParticleSystem FootstepParticles;

    // Vectors for collision detection
    private Vector2 playerSize;
    private Vector2 playerColliderOffset;
    private Vector2 CollisionSizeJump;
    private Vector2 CollisionsSizeWall;

    // Current Character Information
    private bool Grounded = false;
    private bool GroundedLastFrame = false;
    private bool CollidingWithWall = false;
    private bool JumpRequest = false;
    private bool JumpHeld = false;
    private bool JumpBufferUsed = false;
    private float HorizontalMovement = 0;

    // Timers to handle jumping
    private float HangCounter;
    private float JumpBufferCounter;
    private float JumpCooldownCounter;
    private float CurrentExtraJumpTime;

    // Amim
    private float IdleCounter;

    // Particles
    private ParticleSystem.EmissionModule DustEmissionModule;

    //Respawn
    private SpawnPoint CurrentSpawnPoint;

    // Controller
    private bool JoyStickConnected = true;
    private float JoyStickConnectedCheck = 1f;
    private float currentJoyStickConnectedCheck = 01f; // Forces it to check at start;


    private void Awake()
    {
        playerSize = myCollider.size;
        playerColliderOffset = myCollider.offset;

        CollisionSizeJump = new Vector2(myCollider.size.x - m_groundDetectionPadding, m_groundDetectionPadding);
        CollisionsSizeWall = new Vector2(m_wallDetectionPadding, myCollider.size.y);

        DustEmissionModule = FootstepParticles.emission;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            JumpRequest = true;
        }

        JumpHeld = Input.GetButton("Jump");

        if (JoyStickConnected)
        {
            HorizontalMovement = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            HorizontalMovement = Input.GetAxis("Horizontal");
        }

        if (currentJoyStickConnectedCheck >= JoyStickConnectedCheck)
        {
            currentJoyStickConnectedCheck = 0f;
            JoyStickConnected = CheckJoyStickConnected();
        }

        currentJoyStickConnectedCheck += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        GroundedLastFrame = Grounded;
        Grounded = IsGrounded();

        if (!InMenus)
        {
            if (PlayerHasControl)
            {
                Idle();
                Flip();
                Jump();
            }

            Run();
            CheckForDeath();
        }
        else
        {
            myAnimator.SetBool("IsSleeping", true);
        }
    }

    private void Run()
    {
        Vector2 direction = Vector2.zero;

        if (HorizontalMovement > 0)
        {
            direction = Vector2.right;
        }
        else if (HorizontalMovement < 0)
        {
            direction = Vector2.left;
        }

        if (direction != Vector2.zero)
        {
            Vector2 boxCenter = (Vector2)transform.position + direction * (playerSize.x) * 0.5f;
            boxCenter += playerColliderOffset;

            CollidingWithWall = Physics2D.OverlapBox(boxCenter, CollisionsSizeWall, 0, m_wallsLayerMask);
            DebugCollision(boxCenter, CollisionsSizeWall, !CollidingWithWall);

            if (!CollidingWithWall)
            {
                Vector2 playerVelocity = new Vector2(HorizontalMovement * m_speed, myRigidbody.velocity.y);
                myRigidbody.velocity = playerVelocity;

                myAnimator.SetBool("IsRunning", true);

                if (Grounded)
                {
                    DustEmissionModule.rateOverTime = 35;
                }
                else
                {
                    DustEmissionModule.rateOverTime = 0;
                }
            }
            else
            {
                myAnimator.SetBool("IsRunning", false);
                DustEmissionModule.rateOverTime = 0;
            }
        }
        else
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            myAnimator.SetBool("IsRunning", false);
            DustEmissionModule.rateOverTime = 0;
        }
    }

    private void Jump()
    {
        HangCounter = (Grounded) ? m_hangTime : HangCounter - Time.deltaTime;
        JumpBufferCounter = (JumpRequest) ? m_jumpBuffer : JumpBufferCounter - Time.deltaTime;
        JumpCooldownCounter -= Time.deltaTime;
    
        // bug here
        if (JumpRequest && !Grounded)
        {
            JumpBufferUsed = true;
        }

        if (JumpHeld)
        {
            CurrentExtraJumpTime -= Time.deltaTime;
        }
        else
        {
            CurrentExtraJumpTime = 0;
        }

        // Prevents accidental double jump, coyote time, buffer on jumping before ground is touched
        if (HangCounter > 0 && JumpBufferCounter > 0 && JumpCooldownCounter <= 0)
        {
            HangCounter = 0;
            JumpBufferCounter = 0;
            JumpCooldownCounter = m_jumpCooldown;
            CurrentExtraJumpTime = m_ExtraJumpTime;

            if (JumpBufferUsed)
            {
                myAnimator.SetTrigger("IsLanding");
                LandingParticles.Play();
            }

            JumpBufferUsed = false;
            myRigidbody.AddForce(new Vector2(myRigidbody.velocity.x, m_jumpForce), ForceMode2D.Impulse);
        }
        else
        {
            JumpBufferUsed = false;
        }

        if (Grounded)
        {
            myAnimator.SetBool("IsJumping", false);
        }
        else
        {
            myAnimator.SetBool("IsJumping", true);
        }

        if (Grounded && !GroundedLastFrame)
        {
            myAnimator.SetTrigger("IsLanding");

            if (myRigidbody.velocity.y < 0.01f)
                LandingParticles.Play();
        }

        if (myRigidbody.velocity.y < 0.0001f)
        {
            myRigidbody.gravityScale = m_fallMultiplier;
        }
        else if (myRigidbody.velocity.y > 0f)
        {
            if (CurrentExtraJumpTime > 0.0001f)
            {
                myRigidbody.gravityScale = m_highJumpMultiplier;
            }
            else
            {
                myRigidbody.gravityScale = m_lowJumpMultiplier;
            }
        }
        else
        {
            myRigidbody.gravityScale = 1f;
        }

        JumpRequest = false;
    }

    private bool IsGrounded()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (playerSize.y) * 0.5f;
        boxCenter += playerColliderOffset;

        bool grounded = Physics2D.OverlapBox(boxCenter, CollisionSizeJump, 0, m_groundLayerMask);
        DebugCollision(boxCenter, CollisionSizeJump, grounded);
        return grounded;
    }

    private void DebugCollision(Vector2 boxCenter, Vector2 boxSize, bool successful)
    {
        Vector2 t_tL = boxCenter + new Vector2(-boxSize.x * 0.5f, boxSize.y * 0.5f);
        Vector2 t_tR = boxCenter + new Vector2(boxSize.x * 0.5f, boxSize.y * 0.5f);
        Vector2 t_bL = boxCenter + new Vector2(-boxSize.x * 0.5f, -boxSize.y * 0.5f);
        Vector2 t_bR = boxCenter + new Vector2(boxSize.x * 0.5f, -boxSize.y * 0.5f);
        Color t_lineColor = (successful) ? Color.green : Color.red;

        Debug.DrawLine(t_tL, t_tR, t_lineColor);
        Debug.DrawLine(t_bL, t_bR, t_lineColor);
        Debug.DrawLine(t_tL, t_bL, t_lineColor);
        Debug.DrawLine(t_tR, t_bR, t_lineColor);
    }

    private void Flip()
    {
        if (HorizontalMovement > 0)
        {
            Vector3 newScale = new Vector3(1 * Mathf.Abs(this.transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
        }
        else if (HorizontalMovement < 0)
        {
            Vector3 newScale = new Vector3(-1 * Mathf.Abs(this.transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
        }
    }

    private void Idle()
    {
        if (JumpRequest == false && HorizontalMovement == 0)
        {
            IdleCounter += Time.deltaTime;

            if (IdleCounter >= IdleTimer)
            {
                myAnimator.SetBool("IsIdle", true);
            }
        }
        else
        {
            if (!IsMenuCat)
            {
                Timer.StartTimer();
            }

            myAnimator.SetBool("IsIdle", false);
            IdleCounter = 0;
        }
    }

    private void CheckForDeath()
    {
        if (transform.position.y < MinYValue && coroutine == null)
        {
            PlayerHasControl = false;
            myRigidbody.simulated = false;
            coroutine = PlayerRespawnSequence();
            StartCoroutine(coroutine);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name.Equals("Platform"))
        {
            transform.parent = col.transform;
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 13 && coroutine == null)
        {
            PlayerHasControl = false;
            myAnimator.SetBool("Injured", true);

            myRigidbody.velocity = Vector2.zero;
            myRigidbody.gravityScale = m_lowJumpMultiplier;
            Vector2 knockbackforce = (transform.position - col.transform.position) * ForceModifier;
            myRigidbody.AddForce(new Vector2(knockbackforce.x, DamageUplift), ForceMode2D.Impulse);

            DamageType damageType = DamageType.Falling;

            if (col.gameObject.tag == "Raven")
            {
                Vector3 halfwayPoint = Vector3.Lerp(col.transform.position, transform.position, 0.5f);
                Timer.PlayerTakenDamage(damageType, halfwayPoint);
                damageType = DamageType.Raven;
            }
            else if (col.gameObject.tag == "Hedgehog")
            {
                Timer.PlayerTakenDamage(damageType, col.transform.position + Vector3.up);
                damageType = DamageType.Hedgehog;
            }

            coroutine = PlayerDamageSequence(damageType);
            StartCoroutine(coroutine);
        }

        if (col.gameObject.layer == 15)
        {
            CurrentSpawnPoint = col.gameObject.GetComponent<SpawnPoint>();
            CurrentSpawnPoint.SetSpawnPoint();
        }
    }

    private IEnumerator PlayerRespawnSequence()
    {
        ResetCharacterPhysics();
        yield return new WaitForSeconds(DamageTime);
        gameObject.transform.position = CurrentSpawnPoint.SpawnPosition.position;
        Timer.PlayerTakenDamage(DamageType.Falling, gameObject.transform.position + Vector3.right);
        CurrentSpawnPoint.PlayerRespawned();
        myRenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);
        myRenderer.enabled = true;
        yield return new WaitForSeconds(0.4f);
        myRigidbody.simulated = true;
        PlayerHasControl = true;
        myRigidbody.velocity = Vector2.zero;

        coroutine = null;
    }

    private IEnumerator PlayerDamageSequence(DamageType damageType)
    {
        ResetCharacterPhysics();

        myRenderer.material.SetFloat("_FlashAmount", 1);
        yield return new WaitForSeconds(0.1f);

        myRenderer.material.SetFloat("_FlashAmount", 0);
        float currentDamagetime = 0f;

        while (currentDamagetime < DamageTime - 0.1f)
        {
            currentDamagetime += Time.deltaTime;
            yield return null;
        }

        myRenderer.color = Color.white;
        myAnimator.SetBool("Injured", false);
        PlayerHasControl = true;
        myRigidbody.gravityScale = 1f;

        coroutine = null;
    }

    private void ResetCharacterPhysics()
    {
        HangCounter = 0;
        JumpBufferCounter = 0;
        JumpCooldownCounter = m_jumpCooldown;
        CurrentExtraJumpTime = m_ExtraJumpTime;
        Grounded = false;
    }

    public void SetInMenus()
    {
        myRigidbody.velocity = Vector2.zero;
        InMenus = true;
    }

    public void SetOutMenus()
    {
        InMenus = false;
        myAnimator.SetBool("IsSleeping", false);
    }

    private bool CheckJoyStickConnected()
    {
        //Get Joystick Names
        string[] temp = Input.GetJoystickNames();

        //Check whether array contains anything
        if (temp.Length > 0)
        {
            //Iterate over every element
            for (int i = 0; i < temp.Length; ++i)
            {
                //Check if the string is empty or not
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    //Not empty, controller temp[i] is connected
                    Debug.Log("Controller " + i + " is connected using: " + temp[i]);
                    return true;
                }
                else
                {
                    //If it is empty, controller i is disconnected
                    //where i indicates the controller number
                    Debug.Log("Controller: " + i + " is disconnected.");
                    return false;
                }
            }
        }

        return false;
    }
}