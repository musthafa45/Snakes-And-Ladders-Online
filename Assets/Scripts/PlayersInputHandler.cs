using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersInputHandler : NetworkBehaviour
{
    public static PlayersInputHandler Instance;

    private const short player1_Id = 1;
    private const short player2_Id = 2;

    private int toFirstPlay_Player_Id;

    private NetworkVariable<short> toFirstPlay_Player_Id_Netwrk_variable = new NetworkVariable<short>();

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        //toFirstPlay_Player_Id = GetTossWinPlayer();
        if(IsServer)
        {
            toFirstPlay_Player_Id_Netwrk_variable.Value = GetTossWinPlayer();
        }
    }

    public short GetTossWinPlayer()
    {
        return (short)UnityEngine.Random.Range(player1_Id,player2_Id); // Because It Cannot Select Last That Means 2 So +1
    }

    public short GetFirstPlayPlayerId()
    {
        //return toFirstPlay_Player_Id;
        return toFirstPlay_Player_Id_Netwrk_variable.Value;
    }
}
