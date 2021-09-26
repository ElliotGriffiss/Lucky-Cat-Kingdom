using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataClasses;

public class Coin : MonoBehaviour
{
    [SerializeField] private CoinType Type;


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            gameObject.SetActive(false);

            TimeManager.Instance.CoinCollected(Type);
        }
    }
}
