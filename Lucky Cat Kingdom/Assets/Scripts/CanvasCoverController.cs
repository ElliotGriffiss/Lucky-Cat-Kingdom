using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCoverController : MonoBehaviour
{
    [Header("Scene Refernces")]
    [SerializeField] private GameObject LeftPanel;
    [SerializeField] private GameObject RightPanel;
    [SerializeField] private GameObject PanelParent;

    [Header("Animation Settings")]
    [SerializeField] private float OpenTime = 1f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform LeftPanelOffSceenPosition;
    [SerializeField] private RectTransform LeftPanelOnSceenPosition;

    [SerializeField] private RectTransform RightPanelOffSceenPosition;
    [SerializeField] private RectTransform RightPanelOnSceenPosition;

    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();

    public IEnumerator MoveOnScreen()
    {
        PanelParent.SetActive(true);

        float currentOpenTime = 0;

        while (currentOpenTime < OpenTime)
        {
            LeftPanel.transform.position = Vector2.LerpUnclamped(LeftPanelOffSceenPosition.position, LeftPanelOnSceenPosition.position, PanelAnimationCurve.Evaluate(currentOpenTime / OpenTime));
            RightPanel.transform.position = Vector2.LerpUnclamped(RightPanelOffSceenPosition.position, RightPanelOnSceenPosition.position, PanelAnimationCurve.Evaluate(currentOpenTime / OpenTime));

            yield return waitForFrameEnd;
            currentOpenTime += Time.unscaledDeltaTime;
        }

        LeftPanel.transform.position = LeftPanelOnSceenPosition.position;
        RightPanel.transform.position = RightPanelOnSceenPosition.position;
    }

    public IEnumerator MoveOffScreen()
    {
        float currentOpenTime = 0;

        while (currentOpenTime < OpenTime)
        {
            LeftPanel.transform.position = Vector2.LerpUnclamped(LeftPanelOnSceenPosition.position, LeftPanelOffSceenPosition.position, PanelAnimationCurve.Evaluate(currentOpenTime / OpenTime));
            RightPanel.transform.position = Vector2.LerpUnclamped(RightPanelOnSceenPosition.position, RightPanelOffSceenPosition.position, PanelAnimationCurve.Evaluate(currentOpenTime / OpenTime));

            yield return waitForFrameEnd;
            currentOpenTime += Time.unscaledDeltaTime;
        }

        LeftPanel.transform.position = LeftPanelOffSceenPosition.position;
        RightPanel.transform.position = RightPanelOffSceenPosition.position;

        PanelParent.SetActive(false);
    }


}
