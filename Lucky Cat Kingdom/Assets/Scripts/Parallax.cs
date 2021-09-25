using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform Camera;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [Space]
    [SerializeField] private float ParallaxEffect;

    private Vector3 LastCameraPos;
    private float textureUnitSize;

    private void Start()
    {
        LastCameraPos = Vector2.zero;

        Texture2D texture = SpriteRenderer.sprite.texture;
        textureUnitSize = texture.width / SpriteRenderer.sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMove = Camera.transform.position - LastCameraPos;
        deltaMove.y = 0f;
        transform.position += deltaMove * ParallaxEffect;

        LastCameraPos = Camera.transform.position;

        if (Mathf.Abs(Camera.transform.position.x - transform.position.x) >= textureUnitSize)
        {
            float offset = Camera.transform.position.x - transform.position.x % textureUnitSize;
            transform.position = new Vector3(Camera.transform.position.x + offset, transform.position.y);
        }
    }
}
