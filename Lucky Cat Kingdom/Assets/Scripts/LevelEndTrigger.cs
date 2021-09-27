using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private GameCompletedCanvas GameCompletedCanvas;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            GameCompletedCanvas.TriggerGameCompletedCanvas();
        }
    }
}
