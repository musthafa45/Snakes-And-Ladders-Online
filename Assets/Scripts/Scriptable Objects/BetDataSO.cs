using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BetData")]
public class BetDataSO : ScriptableObject
{
    public List<BetData> BetDataSOList; 

    [System.Serializable]
    public class BetData
    {
        public string GameMode;
        public float WinAmount;
        public float EntryAmount;
        public Sprite GameLogoSprite;
    }
}
