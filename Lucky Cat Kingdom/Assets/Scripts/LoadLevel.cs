using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadLevel : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private CanvasCoverController CanvasCoverController;

    [Header("Animation Settings")]
    [SerializeField] private float UITickPause = 0.5f;

    private IEnumerator coroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetAxis("Submit") == 1)
        {
            if (coroutine == null)
            {
                coroutine = LoadLevelSequence();
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator LoadLevelSequence()
    {
        yield return CanvasCoverController.MoveOnScreen();
        yield return new WaitForSeconds(UITickPause);
        SceneManager.LoadScene("SampleScene");
        coroutine = null;
    }
}
