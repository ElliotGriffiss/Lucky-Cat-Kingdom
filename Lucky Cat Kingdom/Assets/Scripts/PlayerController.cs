using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
    [SerializeField] private float ForceModifier = 1f;
    private IEnumerator coroutine;
    private bool PlayerHasControl = true;

    [Header("Scene References")]
    [SerializeField] private SpriteRenderer myRenderer;
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private BoxCollider2D myCollider;
    [Space]
    [SerializeField] private ParticleSystem LandingParticles;
    [SerializeField] private ParticleSystem FootstepParticles;

    [SerializeField] private ParticleSystem BloodParticles;

    // Vectors for collision detection
    private Vector2 playerSize;
    private Vector2 playerColliderOffset;
    private Vector2 CollisionSizeJump;
    private Vector2 CollisionsSizeWall;

    // Current Character Information
    private bool Grounded = false;
    private bool GroundedLastFrame = false;
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


    private void Awake()
    {
        playerSize = myCollider.size;
        playerColliderOffset = myCollider.offset;

        CollisionSizeJump = new Vector2(myCollider.size.x - m_groundDetectionPadding, m_groundDetectionPadding);
        CollisionsSizeWall = new Vector2(m_wallDetectionPadding, myCollider.size.y - m_wallDetectionPadding);

        DustEmissionModule = FootstepParticles.emission;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            JumpRequest = true;
        }

        JumpHeld = Input.GetButton("Jump");

        HorizontalMovement = Input.GetAxis("Horizontal");
    }



    private void FixedUpdate()
    {
        if (PlayerHasControl)
        {
            GroundedLastFrame = Grounded;
            Grounded = IsGrounded();

            Idle();
            Flip();
            Run();
            Jump();
        }

       // if (Vector3.Distance(groundedStart, groundedEnd) < 0.01f)
       // {
            // play particle effect
       // }
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

            bool collidingWithWall = Physics2D.OverlapBox(boxCenter, CollisionsSizeWall, 0, m_wallsLayerMask);
            DebugCollision(boxCenter, CollisionsSizeWall, !collidingWithWall);

            if (!collidingWithWall)
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
            Debug.Log("Used");
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
                Debug.Log("played");
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
            myAnimator.SetBool("IsIdle", false);
            IdleCounter = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name.Equals("Platform"))
        {
            transform.parent = col.transform;
        }

        if (col.gameObject.tag == "DamageBlock" && coroutine == null)
        {
            PlayerHasControl = false;
            myRigidbody.freezeRotation = false;
            myAnimator.SetBool("Dead", true);
            BloodParticles.Play();
            myRigidbody.AddForceAtPosition(GenerateRandomForce(), col.gameObject.transform.position, ForceMode2D.Impulse);
            coroutine = PlayerDeathSequence();
            StartCoroutine(coroutine);
        }

        if (coroutine != null)
        {
            myRigidbody.AddForceAtPosition(GenerateRandomForce(), col.gameObject.transform.position, ForceMode2D.Impulse);
            BloodParticles.Play();
        }
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "DamageBlock" && coroutine == null)
        {
            PlayerHasControl = false;
            myRigidbody.freezeRotation = false;
            myAnimator.SetBool("Dead", true);
            coroutine = PlayerDeathSequence();
            StartCoroutine(coroutine);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.name.Equals("Platform"))
        {
            transform.parent = null;
        }
    }

    private IEnumerator PlayerDeathSequence()
    {
        yield return new WaitForSeconds(1f);
        myAnimator.SetBool("Dead", false);
        PlayerHasControl = true;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        myRigidbody.freezeRotation = true;
        myRigidbody.velocity = Vector2.zero;
        BloodParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        Vector3 oldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        coroutine = null;
    }

    private Vector2 GenerateRandomForce()
    {
        return new Vector2(UnityEngine.Random.Range(-ForceModifier, ForceModifier), UnityEngine.Random.Range(-ForceModifier, ForceModifier));
    }
}