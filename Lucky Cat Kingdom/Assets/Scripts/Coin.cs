using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataClasses;

public class Coin : MonoBehaviour
{
    [SerializeField] private TimeManager Timer;
    [SerializeField] private CoinType Type;


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            gameObject.SetActive(false);
            Timer.CoinCollected(Type, gameObject.transform.position);
        }
    }
}
