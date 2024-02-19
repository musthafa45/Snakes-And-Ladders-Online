using QFSW.QC;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyTest : MonoBehaviour
{
    private Lobby hostLobby;

    private float heartBeatTimer = 0;
    private float heartBeatTimerMax = 15f;
    private async void Start()
    {
       await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Authendication Success {AuthenticationService.Instance.PlayerId}");
        };

       await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void Update()
    {
        if( hostLobby != null )
        {
            heartBeatTimer += Time.deltaTime;
            if(heartBeatTimer > heartBeatTimerMax)
            {
                heartBeatTimer = 0;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    [Command]
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "FuckingLobby";
            int maxPlayer = 4;

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer);
            Debug.Log($"Lobby Name {hostLobby.Name} And Lobby MaxPlayers {hostLobby.MaxPlayers}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    [Command]
    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 10,

                Filters = new List<QueryFilter> { 
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }

            };

           QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
           Debug.Log($"Lobby Count{ queryResponse.Results.Count}");

            foreach(Lobby lobby in queryResponse.Results)
            {
                Debug.Log($"LobbyName = {lobby.Name} And Lobby max player{lobby.MaxPlayers}");
            }
        }
        catch (LobbyServiceException e)
        { 
            Debug.Log(e);
        }
    }

    [Command]
    public async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

            Debug.Log("Joined Lobby Name = " + lobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
