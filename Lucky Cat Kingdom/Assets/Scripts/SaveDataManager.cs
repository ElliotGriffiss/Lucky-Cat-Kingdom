using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataClasses;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;
    public AllScoreBoardData AllData;

    private void Awake()
    {
        if (SaveDataManager.Instance != null && SaveDataManager.Instance != this)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            SaveDataManager.Instance = this;
            DontDestroyOnLoad(gameObject);
            string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/ScoreBoardData.json");
            AllData = JsonUtility.FromJson<AllScoreBoardData>(json);
        }
    }

    public void AddData(ScoreBoardData scoreBoardData)
    {
        AllData.Data.Add(scoreBoardData);
        string json = JsonUtility.ToJson(AllData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/ScoreBoardData.json", json);
    }

    public void SortAndReturnData()
    {

    }
}
