using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataClasses;
using System.Linq;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;
    private AllScoreBoardData AllData;

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

            if (System.IO.File.Exists(Application.persistentDataPath + "/ScoreBoardData.json"))
            {
                string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/ScoreBoardData.json");
                AllData = JsonUtility.FromJson<AllScoreBoardData>(json);
            }
            else
            {
                AllData = new AllScoreBoardData();
                AllData.Data = new List<ScoreBoardData>();
            }

        }
    }

    public void AddData(ScoreBoardData scoreBoardData)
    {
        AllData.Data.Add(scoreBoardData);

        AllData.Data = AllData.Data.OrderBy(x => x.Score).ToList();

        string json = JsonUtility.ToJson(AllData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/ScoreBoardData.json", json);
    }

    public List<ScoreBoardData> GetSortedData()
    {
        return AllData.Data;
    }

    // The Location of the save file.
    //%userprofile%\AppData\LocalLow\<companyname>\
}
