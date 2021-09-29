using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rigidbody2D;
    [SerializeField] private Transform StartPos;
    [SerializeField] private Transform EndPos;
    [Space]
    [SerializeField] private float movementSpeed;

    private bool Moving = false;

    private void FixedUpdate()
    {
        if (Moving && Vector2.Distance(gameObject.transform.position, EndPos.position) > 0.1f)
        {
             Rigidbody2D.MovePosition(gameObject.transform.position += Vector3.right * movementSpeed * Time.fixedDeltaTime);
        }
        else if (!Moving && Vector2.Distance(gameObject.transform.position, StartPos.position) > 0.1f)
        {
            Rigidbody2D.MovePosition(gameObject.transform.position += Vector3.left * movementSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11 && col.gameObject.transform.position.y > gameObject.transform.position.y)
        {
            col.collider.transform.parent.SetParent(transform);
            Moving = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
        {
            col.collider.transform.parent.SetParent(null);
            Moving = false;
        }
    }
}
