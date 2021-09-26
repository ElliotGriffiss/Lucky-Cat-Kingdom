using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Animator Animator;

    [Header("Settings")]
    [SerializeField] private Transform StartPos;
    [SerializeField] private Transform EndPos;
    [Space]
    [SerializeField] private float IdleSpeed;
    [SerializeField] private float ChaseSpeed;
    [SerializeField] private float MaxFollowDistance;

    private Transform Target;
    private Vector3 StartFollowPoint;

    private RavenState CurrentRavenState = RavenState.Flying_right;
    private RavenState LastRavenState;

    enum RavenState
    {
        Flying_left,
        Flying_right,
        Chasing,
    }

    private void Start()
    {
        StartFollowPoint = Vector3.Lerp(EndPos.position, StartPos.position, 0.5f);
    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;

        if (CurrentRavenState == RavenState.Flying_left)
        {
            direction = (StartPos.transform.position - gameObject.transform.position).normalized;
            Vector3 playerVelocity = direction * IdleSpeed * Time.deltaTime;
            gameObject.transform.position += playerVelocity;

            if (Vector3.Distance(transform.position, StartPos.position) < 0.25f)
            { 
                CurrentRavenState = RavenState.Flying_right;
                Animator.SetBool("IsChasing", false);
            }

            DebugPath(direction);
        }
        else if (CurrentRavenState == RavenState.Flying_right)
        {
            direction = (EndPos.transform.position - gameObject.transform.position).normalized;
            Vector3 playerVelocity = direction * IdleSpeed * Time.deltaTime;
            gameObject.transform.position += playerVelocity;

            if (Vector3.Distance(transform.position, EndPos.position) < 0.25f)
            {
                CurrentRavenState = RavenState.Flying_left;
                Animator.SetBool("IsChasing", false);
            }

            DebugPath(direction);
        }
        else if (CurrentRavenState == RavenState.Chasing)
        {
            direction = (Target.position - gameObject.transform.position).normalized;
            Vector3 playerVelocity = direction * ChaseSpeed * Time.deltaTime;
            gameObject.transform.position += playerVelocity;

            if (Vector3.Distance(gameObject.transform.position, StartFollowPoint) > MaxFollowDistance)
            {
                Target = null;
                CurrentRavenState = LastRavenState;
                Animator.SetBool("IsChasing", false);
            }

            DebugPath(direction);
        }

        Flip(direction);
    }

    private void Flip(Vector3 direction)
    {
        if (direction.x < 0)
        {
            Vector3 newScale = new Vector3(-1 * Mathf.Abs(this.transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
        }
        else
        {
            Vector3 newScale = new Vector3(1 * Mathf.Abs(this.transform.localScale.x), transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Enter");

        if (col.gameObject.layer == 11)
        {
            Target = col.transform;
            Animator.SetBool("IsChasing", true);

            if (CurrentRavenState != RavenState.Chasing)
            {
                LastRavenState = CurrentRavenState;
                CurrentRavenState = RavenState.Chasing;
            }
        }
    }

    private void DebugPath(Vector3 direction)
    {
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + direction * 10, Color.red, 10f);
    }
}
