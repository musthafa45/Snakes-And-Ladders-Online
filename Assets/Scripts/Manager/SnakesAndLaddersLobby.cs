using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class SnakesAndLaddersLobby : MonoBehaviour
{
    private string playerName;
    private Lobby joinedLobby;

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

        CreateOrJoinLobby();
    }


    private async void CreateOrJoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await GetAvailableLobbies();

            if(queryResponse.Results.Count > 0)
            {
                Lobby lobby = await  Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
                joinedLobby = lobby;

                SnakesAndLaddersMultiplayer.Instance.StartClient();

                Debug.Log("Joined Already Created Lobby " + joinedLobby.Name);
            }
            else
            {
                Debug.LogWarning("Cant Found One Lobby To Join & try Creating Lobby");

                string lobbyName = "Fucking Lobby";
                int maxPlayer = 2;

                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    Player = GetPlayer(),
                };

                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);

                SnakesAndLaddersMultiplayer.Instance.StartHost();

                Debug.Log("Created Lobby And Joined " + joinedLobby.Name);
            }
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<QueryResponse> GetAvailableLobbies()
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
                    new QueryOrder(asc: false, field: QueryOrder.FieldOptions.Created)
                }

            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            return queryResponse;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return default;
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
}
