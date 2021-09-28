using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartController : MonoBehaviour
{
    [SerializeField] private CanvasCoverController CanvasCoverController;
    [SerializeField] private PlayerController Player;
    [Space]
    [SerializeField] private float StartDelay = 0.5f;

    private void Start()
    {
        Player.SetInMenus();
        StartCoroutine(OpenMenus());
    }

    private IEnumerator OpenMenus()
    {
        yield return CanvasCoverController.MoveOffScreen();
        Player.SetOutMenus();
    }
}
