using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartManager : MonoBehaviour
{
    public static GameStartManager Instance {  get; private set; }

    private List<PlayerReadyData> playerReadyDatas = new List<PlayerReadyData>();
    private int maxPlayersCount = 2;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerReady(ulong playerId)
    {
        PlayerReadyData playerReadyData = playerReadyDatas.Find(pdata => pdata.playerId == playerId); // Player Data Exist

        if(playerReadyData != null)
        {
            playerReadyData.IsReady = true;
            CheckIfMatchCanStart();
        }
        else
        {
            PlayerReadyData readyData = new PlayerReadyData();
            readyData.playerId = playerId;
            readyData.IsReady = true;

            playerReadyDatas.Add(readyData);
            CheckIfMatchCanStart();
        }
    }

    private void CheckIfMatchCanStart()
    {
        Debug.Log("Callled");
        if(playerReadyDatas.Count == maxPlayersCount && playerReadyDatas.TrueForAll(pData => pData.IsReady))
        {
            SceneManager.LoadScene("MultiplayerLocalAndCom");
        }
    }

    public void SetPlayerNotReady(ulong playerId)
    {

    }
}

[Serializable]
public class PlayerReadyData
{
    public ulong playerId;
    public bool IsReady;
}
