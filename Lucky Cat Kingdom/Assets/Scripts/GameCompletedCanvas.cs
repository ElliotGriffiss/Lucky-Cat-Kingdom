using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameCompletedCanvas : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private CanvasCoverController CanvasCoverController;
    [SerializeField] private TimeManager Timer;
    [SerializeField] private PlayerController Player;
    [SerializeField] private GameObject PanelParent;

    [Header("UI Components")]
    [SerializeField] private Image Background;
    [SerializeField] private TextMeshProUGUI CoinsCollected;
    [SerializeField] private TextMeshProUGUI TimesHit;
    [Space]
    [SerializeField] private TextMeshProUGUI FinalTime;

    [Header("Animation Settings")]
    [SerializeField] private float UITickRate = 0.08f;
    [SerializeField] private float UITickPause = 0.5f;

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

        yield return CanvasCoverController.MoveOnScreen();
        PanelParent.SetActive(true);
        Timer.gameObject.SetActive(false);
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

        if (ButtonPressed == false)
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

        if (ButtonPressed == false)
            yield return new WaitForSeconds(UITickPause);

        FinalTime.text = "Final Time: " + Timer.GetTime().ToString("0.0") + "s";

        ButtonPressed = false;

        while (ButtonPressed == false)
        {
            yield return null;
        }

        yield return CanvasCoverController.MoveOnScreen();
        yield return new WaitForSeconds(UITickPause);

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
