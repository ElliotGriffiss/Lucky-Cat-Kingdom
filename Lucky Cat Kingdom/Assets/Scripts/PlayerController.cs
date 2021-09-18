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
    [Space]
    [SerializeField] private float m_hangTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float m_jumpBuffer = 0.2f;
    [SerializeField] private float m_jumpCooldown = 0.2f;
    [SerializeField] private float m_fallMultiplier = 2.5f;
    [SerializeField] private float m_lowJumpMultiplier = 2.5f;
    [SerializeField] private float m_groundDetectionPadding = 0.1f;
    [SerializeField] private float m_wallDetectionPadding = 0.05f;
    [Space]
    [SerializeField] private LayerMask platformLayerMask;

    // Death
    private bool PlayerHasControl = true;
    [SerializeField] private float ForceModifier = 1f;
    private IEnumerator coroutine;


    [Header("Scene References")]
    [SerializeField] private SpriteRenderer myRenderer;
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private BoxCollider2D myCollider;
    [SerializeField] private ParticleSystem BloodParticles;

    private float HangCounter;
    private float JumpBufferCounter;
    private float JumpCooldownCounter;

    private Vector2 playerSize;
    private Vector2 playerColliderOffset;
    private Vector2 CollisionSizeJump;
    private Vector2 CollisionsSizeWall;

    private bool Grounded = false;
    private bool JumpRequest = false;
    private float HorizontalMovement = 0;

    private void Awake()
    {

    }

    private void Update()
    {
        playerSize = myCollider.size;
        playerColliderOffset = myCollider.offset;

        CollisionSizeJump = new Vector2(myCollider.size.x - m_groundDetectionPadding, m_groundDetectionPadding);
        CollisionsSizeWall = new Vector2(m_wallDetectionPadding, myCollider.size.y - m_wallDetectionPadding);

        if (Input.GetButtonDown("Jump"))
        {
            JumpRequest = true;
        }

        HorizontalMovement = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (PlayerHasControl)
        {
            Grounded = IsGrounded();

            Flip();
            Run();
            Jump();
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

            bool collidingWithWall = Physics2D.OverlapBox(boxCenter, CollisionsSizeWall, 0, platformLayerMask);

            Vector2 t_tL = boxCenter + new Vector2(-CollisionsSizeWall.x * 0.5f, CollisionsSizeWall.y * 0.5f);
            Vector2 t_tR = boxCenter + new Vector2(CollisionsSizeWall.x * 0.5f, CollisionsSizeWall.y * 0.5f);
            Vector2 t_bL = boxCenter + new Vector2(-CollisionsSizeWall.x * 0.5f, -CollisionsSizeWall.y * 0.5f);
            Vector2 t_bR = boxCenter + new Vector2(CollisionsSizeWall.x * 0.5f, -CollisionsSizeWall.y * 0.5f);
            Color t_lineColor = (collidingWithWall) ? Color.red : Color.green;

            Debug.DrawLine(t_tL, t_tR, t_lineColor);
            Debug.DrawLine(t_bL, t_bR, t_lineColor);
            Debug.DrawLine(t_tL, t_bL, t_lineColor);
            Debug.DrawLine(t_tR, t_bR, t_lineColor);

            if (!collidingWithWall)
            {
                Vector2 playerVelocity = new Vector2(HorizontalMovement * m_speed, myRigidbody.velocity.y);
                myRigidbody.velocity = playerVelocity;

                myAnimator.SetBool("IsRunning", true);
            }
            else
            {
                myAnimator.SetBool("IsRunning", false);
            }
        }
        else
        {
            myAnimator.SetBool("IsRunning", false);
        }
    }

    private void Jump()
    {
        HangCounter = (Grounded) ? m_hangTime : HangCounter - Time.deltaTime;
        JumpBufferCounter = (JumpRequest) ? m_jumpBuffer : JumpBufferCounter - Time.deltaTime;
        JumpCooldownCounter -= Time.deltaTime;

        // Prevents accidental double jump, coyote time, buffer on jumoing before ground is touched
        if (HangCounter > 0 && JumpBufferCounter > 0 && JumpCooldownCounter <= 0)
        {
            HangCounter = 0;
            JumpBufferCounter = 0;
            JumpCooldownCounter = m_jumpCooldown;

            myAnimator.SetBool("IsJumping", true);
            myRigidbody.AddForce(new Vector2(myRigidbody.velocity.x, m_jumpForce), ForceMode2D.Impulse);
        }

        // less floaty jump
        if (myRigidbody.velocity.y < 0)
        {
            myRigidbody.gravityScale = m_fallMultiplier;
        }
        else if (myRigidbody.velocity.y > 0f && !Input.GetButton("Jump"))
        {
            myRigidbody.gravityScale = m_lowJumpMultiplier;
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

        bool grounded = Physics2D.OverlapBox(boxCenter, CollisionSizeJump, 0, platformLayerMask);

        Vector2 t_tL = boxCenter + new Vector2(-CollisionSizeJump.x * 0.5f, CollisionSizeJump.y * 0.5f);
        Vector2 t_tR = boxCenter + new Vector2(CollisionSizeJump.x * 0.5f, CollisionSizeJump.y * 0.5f);
        Vector2 t_bL = boxCenter + new Vector2(-CollisionSizeJump.x * 0.5f, -CollisionSizeJump.y * 0.5f);
        Vector2 t_bR = boxCenter + new Vector2(CollisionSizeJump.x * 0.5f, -CollisionSizeJump.y * 0.5f);
        Color t_lineColor = (grounded) ? Color.red : Color.green;

        Debug.DrawLine(t_tL, t_tR, t_lineColor);
        Debug.DrawLine(t_bL, t_bR, t_lineColor);
        Debug.DrawLine(t_tL, t_bL, t_lineColor);
        Debug.DrawLine(t_tR, t_bR, t_lineColor);

        if (grounded)
        {
            myAnimator.SetBool("IsJumping", false);
        }

        return grounded;
    }

    private void Flip()
    {
        if (HorizontalMovement > 0)
        {
            var newScale = new Vector3(
            1 * Mathf.Abs(this.transform.localScale.x),
            1 * Mathf.Abs(this.transform.localScale.y),
            1 * Mathf.Abs(this.transform.localScale.y));
            this.transform.localScale = newScale;
        }
        else if (HorizontalMovement < 0)
        {
            var newScale = new Vector3(
            -1 * Mathf.Abs(this.transform.localScale.x),
             1 * Mathf.Abs(this.transform.localScale.y),
             1 * Mathf.Abs(this.transform.localScale.y));
            this.transform.localScale = newScale;

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
            this.transform.parent = null;
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