using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataClasses;

public class TimeManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private TextMeshProUGUI TimerDisplay;

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
    private IEnumerator coroutine;
    private Color StartingColor;

    private float currentTime = 0f;
    private bool timerRunning = false;

    public int CoinsCollected = 0;
    public int Eighty3CoinsCollected = 0;
    public int TimesDamaged = 0;

    private void Start()
    {
        StartingColor = TimerDisplay.color;
    }

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

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void CoinCollected(CoinType type , Vector3 position)
    {
        switch (type)
        {
            case CoinType.Normal:
                CoinsCollected++;
                currentTime += -1;
                EmitParams.position = position;
                ParticlesMinus1.Emit(EmitParams, 1);
                break;
            case CoinType.Eighty3Coin:
                Eighty3CoinsCollected++;
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

        TimesDamaged++;
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
        yield return FlashColor(StartingColor);

        coroutine = null;
    }

    private IEnumerator TakenDamageSequence()
    {
        yield return FlashColor(Red);
        yield return FlashColor(Orange);

        yield return FlashColor(Red);
        yield return FlashColor(Orange);

        yield return FlashColor(Red);
        yield return FlashColor(StartingColor);

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

    public float GetTime()
    {
        return currentTime;
    }
}
