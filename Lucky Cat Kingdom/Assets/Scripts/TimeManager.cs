using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataClasses;

public class TimeManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Text TimerDisplay;

    [Header("Damage Animation Settings")]
    [SerializeField] private Color Red;
    [SerializeField] private Color Orange;

    [Header("Coin Collect Animation Settings")]
    [SerializeField] private Color Green;
    [SerializeField] private Color GreenLight; 
    [Space]
    [SerializeField] private float FlashSpeed;

    [Header("particles")]
    [SerializeField] private ParticleSystem ParticlesPlus1;
    [Space]
    [SerializeField] private ParticleSystem ParticlesMinus1;
    [SerializeField] private ParticleSystem ParticlesMinus3;

    private ParticleSystem.EmitParams EmitParams;

    private float currentTime = 0f;
    private bool timerRunning = false;

    private IEnumerator coroutine;

    private void Update()
    {
        if (timerRunning)
        {
            currentTime += Time.deltaTime;
        }

        TimerDisplay.text = currentTime.ToString("0.0") + "s";
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void CoinCollected(CoinType type , Vector3 position)
    {
        switch (type)
        {
            case CoinType.Normal:
                currentTime += -1;
                EmitParams.position = position;
                ParticlesMinus1.Emit(EmitParams, 1);
                break;
            case CoinType.Eighty3Coin:
                currentTime += -3;
                EmitParams.position = position;
                ParticlesMinus3.Emit(EmitParams, 1);
                break;
        }

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = CoinCollectedSequence();
        StartCoroutine(coroutine);
    }

    public void PlayerTakenDamage(DamageType type, Vector3 position)
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

        EmitParams.position = position;
        ParticlesPlus1.Emit(EmitParams, 1);

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = TakenDamageSequence();
        StartCoroutine(coroutine);
    }

    private IEnumerator CoinCollectedSequence()
    {
        yield return FlashColor(Green);
        yield return FlashColor(GreenLight);

        yield return FlashColor(Green);
        yield return FlashColor(GreenLight);

        yield return FlashColor(Green);
        yield return FlashColor(Color.black);

        coroutine = null;
    }

    private IEnumerator TakenDamageSequence()
    {
        yield return FlashColor(Red);
        yield return FlashColor(Orange);

        yield return FlashColor(Red);
        yield return FlashColor(Orange);

        yield return FlashColor(Red);
        yield return FlashColor(Color.black);

        coroutine = null;
    }

    private IEnumerator FlashColor (Color flashColor)
    {
        float currentFlashTime = 0f;
        Color startingColor = TimerDisplay.color;

        while (currentFlashTime < FlashSpeed)
        {
            TimerDisplay.color = Color.Lerp(startingColor, flashColor, currentFlashTime / FlashSpeed);

            currentFlashTime += Time.deltaTime;
            yield return null;
        }
    }
}
