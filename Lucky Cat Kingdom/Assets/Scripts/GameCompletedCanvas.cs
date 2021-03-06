using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCompletedCanvas : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private NameInputCanvas NameInputCanvas;
    [SerializeField] private CanvasCoverController CanvasCoverController;
    [SerializeField] private TimeManager Timer;
    [SerializeField] private PlayerController Player;
    [SerializeField] private GameObject PanelParent;
    [SerializeField] private GameObject PanelBackground;
    [SerializeField] private GameObject TimerText;
    [Space]
    [SerializeField] private GameObject SkipIndicator;
    [SerializeField] private GameObject ContinueIndicator;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI CoinsCollected;
    [SerializeField] private TextMeshProUGUI TimesHit;
    [Space]
    [SerializeField] private TextMeshProUGUI FinalTime;

    [Header("Animation Settings")]
    [SerializeField] private float UITickRate = 0.08f;
    [SerializeField] private float UITickPause = 0.5f;
    [Space]
    [SerializeField] private float UIMaxTickTime = 3f;

    private Color StartingColor;

    private IEnumerator coroutine;
    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private bool ButtonPressed = false;

    public void TriggerGameCompletedCanvas()
    {
        Timer.StopTimer();
        Player.SetInMenus();

        if (coroutine == null)
        {
            coroutine = GameCompletedSequence();
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator GameCompletedSequence()
    {
        yield return new WaitForSeconds(UITickPause);
        yield return new WaitForSeconds(UITickPause);
        yield return new WaitForSeconds(UITickPause);

        yield return CanvasCoverController.MoveOnScreen();
        PanelParent.SetActive(true);
        TimerText.SetActive(false);
        PanelBackground.SetActive(true);
        SkipIndicator.SetActive(true);
        yield return new WaitForSeconds(UITickPause);
        yield return CanvasCoverController.MoveOffScreen();

        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        int currentTick = 0;
        float currentTickTime = 0;
        int totalCoins = (Timer.Eighty3CoinsCollected * 3) + Timer.CoinsCollected;

        while (currentTick != totalCoins && ButtonPressed == false)
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

        CoinsCollected.text = "-" + totalCoins + "s";
        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        currentTick = 0;
        currentTickTime = 0;
        SkipIndicator.SetActive(true);

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

        TimesHit.text = "+" + Timer.TimesDamaged + "s";
        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        currentTick = 0;
        currentTickTime = 0;
        int intTime = (int)Timer.GetTime();
        SkipIndicator.SetActive(true);

        if (intTime * UITickRate >= UIMaxTickTime)
        {
            UITickRate = UITickRate * 0.5f;
        }

        while (currentTick != intTime && ButtonPressed == false)
        {
            while (currentTickTime < UITickRate && ButtonPressed == false)
            {
                currentTickTime += Time.deltaTime;
                yield return null;
            }

            currentTick++;
            FinalTime.text = currentTick +"."+ Random.Range(0,9);
            currentTickTime = 0;
        }

        FinalTime.text = Timer.GetTime().ToString("0.0");
        yield return new WaitForSeconds(UITickPause);

        ButtonPressed = false;
        SkipIndicator.SetActive(false);
        ContinueIndicator.SetActive(true);

        while (ButtonPressed == false)
        {
            yield return null;
        }

        yield return CanvasCoverController.MoveOnScreen();
        yield return new WaitForSeconds(UITickPause);

        PanelParent.SetActive(false);
        NameInputCanvas.ShowNameInputCanvas(Timer.GetTime());
        coroutine = null;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetAxis("Cancel") == 1)
        {
            ButtonPressed = true;
            SkipIndicator.SetActive(false);
        }
    }
}
