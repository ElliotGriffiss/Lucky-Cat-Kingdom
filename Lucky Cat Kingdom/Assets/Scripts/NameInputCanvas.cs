using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DataClasses;

public class NameInputCanvas : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject PanelParent;
    [SerializeField] private CanvasCoverController CanvasCoverController;
    [Space]
    [SerializeField] private TMP_InputField TMP_InputField;
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private GameObject CallToActionIndicator;

    [Header("Animation Settings")]
    [SerializeField] private float UITickPause = 0.5f;

    private float Score;
    private string Name;

    private IEnumerator coroutine;

    public void ShowNameInputCanvas(float time)
    {
        Score = time;
        TimeText.text = Score.ToString("0.0");

        PanelParent.SetActive(true);

        if (coroutine == null)
        {
            coroutine = NameInputCanvasSequence();
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator NameInputCanvasSequence()
    {
        yield return CanvasCoverController.MoveOffScreen();

        while (!Input.GetKeyDown(KeyCode.Return) || string.IsNullOrEmpty(Name))
        {
            TMP_InputField.ActivateInputField();
            yield return null;

            if (!string.IsNullOrEmpty(Name))
            {
                CallToActionIndicator.SetActive(true);
            }
        }

        // store string here

        yield return CanvasCoverController.MoveOnScreen();

        ScoreBoardData scoreBoardData = new ScoreBoardData();

        scoreBoardData.Score = Score;
        scoreBoardData.Name = Name;

        SaveDataManager.Instance.AddData(scoreBoardData);

        yield return new WaitForSeconds(UITickPause);
        SceneManager.LoadScene("MenuScene");
        coroutine = null;
    }

    public void OnNameEntered(String name)
    {
        Name = name;
    }
}
