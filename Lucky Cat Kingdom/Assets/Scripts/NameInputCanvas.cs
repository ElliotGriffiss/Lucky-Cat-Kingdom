using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NameInputCanvas : MonoBehaviour
{
    [SerializeField] private GameObject PanelParent;
    [SerializeField] private CanvasCoverController CanvasCoverController;
    [Space]
    [SerializeField] private TMP_InputField TMP_InputField;
    [SerializeField] private TextMeshProUGUI TimeText;

    [Header("Animation Settings")]
    [SerializeField] private float UITickPause = 0.5f;

    private float Time;
    private string Name;

    private IEnumerator coroutine;

    public void ShowNameInputCanvas(float time)
    {
        Time = time;
        TimeText.text = Time.ToString("0.0");

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
        TMP_InputField.ActivateInputField();

        while (!Input.GetKeyDown(KeyCode.Return) && String.IsNullOrEmpty(Name))
        {
            yield return null;
        }

        // store string here

        yield return CanvasCoverController.MoveOnScreen();
        yield return new WaitForSeconds(UITickPause);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        coroutine = null;
    }
}
