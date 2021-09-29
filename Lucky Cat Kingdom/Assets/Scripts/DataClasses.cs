using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataClasses
{
    public enum CoinType : byte
    {
        Normal,
        Eighty3Coin,
    }

    public enum DamageType : byte
    {
        Hedgehog,
        Raven,
        Falling,
    }

    [System.Serializable]
    public class ScoreBoardData
    {
        public float Score;
        public string Name;
    }

    [System.Serializable]
    public class AllScoreBoardData
    {
        public List<ScoreBoardData> Data;
    }
}
