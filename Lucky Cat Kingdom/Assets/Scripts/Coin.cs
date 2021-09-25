using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private CoinType Type;

    private enum CoinType : byte
    {
        Normal,
        Eighty3Coin,
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
        {
            gameObject.SetActive(false);
        }
    }
}
