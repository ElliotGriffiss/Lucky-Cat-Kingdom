using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataClasses;

public class SaveDataManager : Singleton<SaveDataManager>
{
    public AllScoreBoardData AllData;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/ScoreBoardData.json");
        AllData = JsonUtility.FromJson<AllScoreBoardData>(json);
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
