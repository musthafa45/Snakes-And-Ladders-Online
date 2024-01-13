using System;
using Unity.Netcode;
using UnityEngine;

public class RandomPlayerSelector : MonoBehaviour
{
    public static RandomPlayerSelector Instance { get; private set; }

    private const int player1_Id = 0;
    private const int player2_Id = 1;

    private int toFirstPlay_Player_Id = -1;

    private void Awake()
    {
        Instance = this;
    }

    public void SetFirstTurnPlayerId()
    {
        toFirstPlay_Player_Id = GetTossWinPlayer();
        Debug.Log("SetFirstTurnPlayerId: " + toFirstPlay_Player_Id);
    }

    private int GetTossWinPlayer()
    {
        int selectedPlayerId = UnityEngine.Random.Range(player1_Id, player2_Id + 1);
        return selectedPlayerId;
    }

    public int GetFirstPlayPlayerId()
    {
        return toFirstPlay_Player_Id;
    }
}
