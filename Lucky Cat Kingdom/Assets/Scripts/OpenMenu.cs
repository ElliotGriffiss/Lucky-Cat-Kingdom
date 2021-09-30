using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private float ReopenTimer = 0.5f;

    private float CurrentReopenTime;
    private bool PlayerOnTrigger;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            PlayerOnTrigger = true;
            Menu.SetActive(true);
        }
    }

    public void Update()
    {
        if (Input.GetAxis("Cancel") == 1)
        {
            if (PlayerOnTrigger && CurrentReopenTime > ReopenTimer)
            {
                CurrentReopenTime = 0;
                Menu.SetActive(!Menu.activeInHierarchy);
            }
        }

        CurrentReopenTime += Time.deltaTime;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            PlayerOnTrigger = false;
            Menu.SetActive(false);
        }
    }
}
