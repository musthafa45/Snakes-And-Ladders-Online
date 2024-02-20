using QFSW.QC;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyTest : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;

    private float heartBeatTimer = 0;
    private float heartBeatTimerMax = 15f;

    private float lobbyPollTimer = 0;
    private float lobbyPollTimerMax = 1.1f;

    private string playerName;
    private async void Start()
    {
       await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Authendication Success {AuthenticationService.Instance.PlayerId}");
        };

       await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Player" + UnityEngine.Random.Range(0, 99);
        Debug.Log(playerName);
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

        if (joinedLobby != null)
        {
            lobbyPollTimer += Time.deltaTime;
            if (lobbyPollTimer > lobbyPollTimerMax)
            {
                lobbyPollTimer = 0;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    [Command]
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "FuckingLobby";
            int maxPlayer = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                //IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode",new DataObject(DataObject.VisibilityOptions.Public,"Arcade") }
                }
                
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);
            Debug.Log($"Lobby Name {hostLobby.Name} And Lobby MaxPlayers {hostLobby.MaxPlayers} And Code {hostLobby.LobbyCode} And " +
                $"Game Mode is {hostLobby.Data["GameMode"].Value}");
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new()
            {
                {"PlayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,playerName.ToString())}
            }
        };
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

            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

            Debug.Log("Joined Lobby Name = " + joinedLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);

            Debug.Log("Joined Lobby Name = " + joinedLobby.Name +" With Lobby Code"+joinedLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };

            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
            PrintPlayers(joinedLobby);
            PrintPlayMode(joinedLobby);
            Debug.Log($"Quick Joined Lobby Name {joinedLobby.Name}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void UpdateLobbyGameState(string gameMode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode",new DataObject(DataObject.VisibilityOptions.Public,gameMode) }
                }
            });

            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    [Command]
    private async void UpdatePlayerName(string playerName)
    {
        try
        {
            this.playerName = playerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id,AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,this.playerName)}
                }
            });
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    [Command]
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
               HostId = joinedLobby.Players[1].Id
            });

            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    [Command]
    private void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    [Command]
    private void PrintPlayMode()
    {
        PrintPlayMode(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Lobby Players");
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    private void PrintPlayMode(Lobby lobby)
    {
        Debug.Log($"PlayMode = {lobby.Data["GameMode"].Value}");
    }

   
}
