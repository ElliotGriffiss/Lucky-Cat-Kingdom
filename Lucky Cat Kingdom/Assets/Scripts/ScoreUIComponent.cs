using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUIComponent : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Time;

    public void SetTextFields(string name, float time)
    {
        Name.text = name;
        Time.text = time.ToString("0.0") + "s";
    }
}
