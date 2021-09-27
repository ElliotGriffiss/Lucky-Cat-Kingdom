using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCompletedCanvas : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private TimeManager Timer;
    [SerializeField] private PlayerController Player;
    [SerializeField] private GameObject MovingParent;

    [Header("UI Components")]
    [SerializeField] private Image Background;
    [SerializeField] private Text CoinsCollected;
    [SerializeField] private Text Eighty3CoinsCollected;
    [SerializeField] private Text TimesHit;
    [Space]
    [SerializeField] private Text FinalTime;

    [Header("Animation Settings")]
    [SerializeField] private float OpenTime = 1f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform OffSceenPosition;
    [SerializeField] private RectTransform OnSceenPosition;
    [SerializeField] private Color BackGroundWhite;
    [SerializeField] private Color BackGroundGreyedOut;
    [Space]
    [SerializeField] private float UITickRate = 0.08f;
    [SerializeField] private float UITickPause = 0.5f;

    private IEnumerator coroutine;
    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private bool ButtonPressed = false;

    public void TriggerGameCompletedCanvas()
    {
        Timer.StopTimer();
        Player.SetInMenus();
        MovingParent.SetActive(true);
        MovingParent.transform.position = OffSceenPosition.position;

        if (coroutine == null)
        {
            coroutine = GameCompletedSequence();
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator GameCompletedSequence()
    {
        yield return new WaitForSeconds(UITickPause);
        float currentOpenTime = 0;

        while (currentOpenTime < OpenTime)
        {
            MovingParent.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(currentOpenTime / OpenTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, PanelAnimationCurve.Evaluate(currentOpenTime / OpenTime));
            yield return waitForFrameEnd;
            currentOpenTime += Time.unscaledDeltaTime;
        }

        MovingParent.transform.position = OnSceenPosition.position;
        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        int currentTick = 0;
        float currentTickTime = 0;

        while (currentTick != Timer.CoinsCollected && ButtonPressed == false)
        {
            while (currentTickTime < UITickRate && ButtonPressed == false)
            {
                currentTickTime += Time.deltaTime;
                yield return null;
            }

            currentTick++;
            CoinsCollected.text = "-" + currentTick + "s";
            currentTickTime = 0;
        }

        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        currentTick = 0;
        currentTickTime = 0;

        while (currentTick != Timer.Eighty3CoinsCollected * 3 && ButtonPressed == false)
        {
            while (currentTickTime < UITickRate && ButtonPressed == false)
            {
                currentTickTime += Time.deltaTime;
                yield return null;
            }

            currentTick++;
            Eighty3CoinsCollected.text = "-" + currentTick + "s";
            currentTickTime = 0;
        }

        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        currentTick = 0;
        currentTickTime = 0;

        while (currentTick != Timer.TimesDamaged && ButtonPressed == false)
        {
            while (currentTickTime < UITickRate && ButtonPressed == false)
            {
                currentTickTime += Time.deltaTime;
                yield return null;
            }

            currentTick++;
            TimesHit.text = "+" + currentTick + "s";
            currentTickTime = 0;
        }

        yield return new WaitForSeconds(UITickPause);

        FinalTime.text = "Final Time: " + Timer.GetTime().ToString("0.0") + "s";

        ButtonPressed = false;

        while (ButtonPressed == false)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        coroutine = null;
    }

    public void OnContinueButtonPressed()
    {
        if (ButtonPressed != true)
        {
            ButtonPressed = true;
        }
    }
}
