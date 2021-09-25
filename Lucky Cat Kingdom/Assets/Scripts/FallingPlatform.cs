using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Rigidbody2D;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform RespawnPoint;
    [SerializeField] private ParticleSystem DirtParticles;
    [Space]
    [SerializeField] private float FallBuffer;
    [SerializeField] private float RespawnTime;
    [Space]
    [SerializeField] private float ShakeAmount;
    [SerializeField] private float ShakeSpeed;
    [Space]
    [SerializeField] private float ReturnTime;
    [SerializeField] private Color StartingColor;
    [SerializeField] private Color InvisableColor;

    private IEnumerator coroutine;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11 && col.gameObject.transform.position.y > gameObject.transform.position.y)
        {
            coroutine = FallSequence();
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator FallSequence()
    {
        DirtParticles.Play();
        float currentBufferTime = 0;

        while (currentBufferTime < FallBuffer)
        {
            Vector2 shakePos = new Vector2(Mathf.Sin(Time.time * ShakeSpeed) * ShakeAmount, 0f);
            SpriteRenderer.transform.localPosition = shakePos;

            currentBufferTime += Time.deltaTime;
            yield return null;
        }

        SpriteRenderer.transform.localPosition = Vector2.zero;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        DirtParticles.Stop();

        yield return new WaitForSeconds(RespawnTime);
        gameObject.transform.position = RespawnPoint.position;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        float currentReturnTime = 0;
        SpriteRenderer.color = InvisableColor;

        while (currentReturnTime < ReturnTime)
        {
            SpriteRenderer.color = Color.Lerp(InvisableColor, StartingColor, currentReturnTime/ReturnTime);
            currentReturnTime += Time.deltaTime;
            yield return null;
        }

        coroutine = null;
    }
}
