using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UITextOutlineAnimator : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private TextMeshProUGUI TargetText;

    [Header("Animation Settings")]
    [SerializeField] private float TimeModifier = 0.5f;
    [SerializeField] private AnimationCurve FlashCurve;
    [SerializeField] private Color FlashBright;
    [SerializeField] private Color FlashDark;

    private float time;

    private void OnEnable()
    {
        time = 0f;
    }

    private void Update()
    {
        TargetText.outlineColor = Color.Lerp(FlashDark, FlashBright, FlashCurve.Evaluate(Mathf.Repeat(time, 1)));
        time += Time.unscaledDeltaTime * TimeModifier;
    }
}
