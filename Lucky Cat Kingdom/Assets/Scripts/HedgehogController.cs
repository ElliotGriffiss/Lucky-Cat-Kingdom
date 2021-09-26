using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Animator Animator;
    [SerializeField] private Rigidbody2D Rigidbody2D;

    [Header("Settings")]
    [SerializeField] private Transform StartPos;
    [SerializeField] private Transform EndPos;
    [SerializeField] private float speed;
    [Header("Spikes")]
    [SerializeField] private float SpikeOpenTime;

    private float currentOpenTime;
    private bool moveToStart = false;

    private HedgehogState CurrentHedgehogState = HedgehogState.Walking_right;

    enum HedgehogState {
        Walking_left,
        Walking_right,
        Spikes,
    }

    private void FixedUpdate()
    {
        Vector2 direction;

        if (CurrentHedgehogState == HedgehogState.Walking_left)
        {
            direction = Vector3.left;
            Vector2 playerVelocity = new Vector2(direction.x * speed, Rigidbody2D.velocity.y);
            Rigidbody2D.AddForce(playerVelocity);

            if (Vector3.Distance(transform.position, StartPos.position) < 0.25f)
            {
                Rigidbody2D.velocity = Vector2.zero;
                currentOpenTime = 0;
                CurrentHedgehogState = HedgehogState.Spikes;
                moveToStart = false;
                Animator.SetBool("HedgehogWalking", false);
            }
        }
        else if (CurrentHedgehogState == HedgehogState.Walking_right)
        {
            direction = Vector3.right;
            Vector2 playerVelocity = new Vector2(direction.x * speed, Rigidbody2D.velocity.y);
            Rigidbody2D.AddForce(playerVelocity);

            if (Vector3.Distance(transform.position, EndPos.position) < 0.25f)
            {
                Rigidbody2D.velocity = Vector2.zero;
                currentOpenTime = 0;
                CurrentHedgehogState = HedgehogState.Spikes;
                moveToStart = true;
                Animator.SetBool("HedgehogWalking", false);
            }
        }
        else if (CurrentHedgehogState == HedgehogState.Spikes)
        {
            if (currentOpenTime < SpikeOpenTime)
            {
                currentOpenTime += Time.deltaTime;
            }
            else
            {
                Animator.SetBool("HedgehogWalking", true);

                if (moveToStart)
                {
                    CurrentHedgehogState = HedgehogState.Walking_left;

                    Vector3 newScale = new Vector3(-1 * Mathf.Abs(this.transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    transform.localScale = newScale;
                }
                else
                {
                    CurrentHedgehogState = HedgehogState.Walking_right;

                    Vector3 newScale = new Vector3(1 * Mathf.Abs(this.transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    transform.localScale = newScale;
                }
            }
        }
    }
}
