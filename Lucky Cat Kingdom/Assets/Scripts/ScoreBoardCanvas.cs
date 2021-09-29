using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataClasses;

public class ScoreBoardCanvas : MonoBehaviour
{
    [SerializeField] private ScoreUIComponent[] ScoreUIComponents;


    private void Start()
    {
        List<ScoreBoardData> data = SaveDataManager.Instance.GetSortedData();

        for (int i = 0; i < data.Count; i++)
        {
            if (i < ScoreUIComponents.Length)
            {
                ScoreUIComponents[i].SetTextFields(data[i].Name, data[i].Score);
            }
        }
    }
}
