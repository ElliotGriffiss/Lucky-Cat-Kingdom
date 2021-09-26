using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataClasses;

public class TimeManager : Singleton<TimeManager>
{
    public Text TimerDisplay;

    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;
        TimerDisplay.text = currentTime.ToString("0.0") + "s";
    }

    public void CoinCollected(CoinType type)
    {
        switch (type)
        {
            case CoinType.Normal:
                currentTime += -1;
                break;
            case CoinType.Eighty3Coin:
                currentTime += -3;
                break;
        }
    }

    public void PlayerTakenDamage(DamageType type)
    {
        switch (type)
        {
            case DamageType.Hedgehog:
                currentTime += 1;
                break;
            case DamageType.Raven:
                currentTime += 1;
                break;
            case DamageType.Falling:
                currentTime += 1;
                break;
        }

        Debug.Log(type);
    }
}
