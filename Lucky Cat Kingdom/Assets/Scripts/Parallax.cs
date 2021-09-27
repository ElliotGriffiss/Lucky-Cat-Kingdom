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
        LastCameraPos = Camera.transform.position;

        Texture2D texture = SpriteRenderer.sprite.texture;
        textureUnitSize = texture.width / SpriteRenderer.sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMove = Camera.transform.position - LastCameraPos;
        deltaMove.y = 0f;
        transform.position += deltaMove * ParallaxEffect;

        LastCameraPos = Camera.transform.position;

        if (transform.position.x - Camera.position.x < -textureUnitSize)
        {
            transform.position = new Vector2(Camera.transform.position.x + textureUnitSize, transform.position.y);
        }
        else if (transform.position.x - Camera.position.x > textureUnitSize)
        {
            transform.position = new Vector2(Camera.transform.position.x - textureUnitSize, transform.position.y);
        }
    }
}
