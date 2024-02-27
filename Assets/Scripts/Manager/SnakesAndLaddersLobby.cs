using QFSW.QC;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class SnakesAndLaddersLobby : MonoBehaviour
{
    public static SnakesAndLaddersLobby Instance { get; private set; }

    private Lobby joinedLobby;

    private float lobbyHeartBeatTimer;
    private float lobbyHeatBeatTimerMax = 15;

    private float lobbyUpdateTimer;
    private float lobbyUpdateTimerMax = 1.5f;

    private void Awake()
    {
        Instance = this;
    }
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Authendication Success {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log(PlayerPrefs.GetString("PlayerName"));

        CreateOrJoinLobby();
    }

    private async void Update()
    {
        if(joinedLobby != null)
        {
            await HandleLobbyHeartBeat();
            await HandleLobbyUpdate();

        }
    }

    private async Task HandleLobbyUpdate()
    {
        lobbyUpdateTimer += Time.deltaTime;

        if (lobbyUpdateTimer > lobbyUpdateTimerMax)
        {
            lobbyUpdateTimer = 0;
            Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
            joinedLobby = updatedLobby;
        }
    }

    private async Task HandleLobbyHeartBeat()
    {
        lobbyHeartBeatTimer += Time.deltaTime;

        if (lobbyHeartBeatTimer > lobbyHeatBeatTimerMax)
        {
            lobbyHeartBeatTimer = 0;
            await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }

    private async void CreateOrJoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await GetAvailableLobbies();

            if(queryResponse.Results.Count > 0)
            {
                QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
                {
                    Player = GetPlayer(),
                };

                Lobby lobby = await  Lobbies.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
                joinedLobby = lobby;

                JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data["RelayCode"].Value);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                SnakesAndLaddersMultiplayer.Instance.StartClient();

                Debug.Log("Joined Already Created Lobby " + joinedLobby.Name);

                ListPlayers();
            }
            else
            {
                Debug.LogWarning("Cant Found One Lobby To Join & try Creating Lobby");

                string lobbyName = "Quick Lobby";
                int maxPlayer = 2;

                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    Player = GetPlayer(),
                };

                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);

                Allocation allocation = await AllocationRelay();

                string relayJoinCode = await GetRelayJoinCode(allocation);

                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {"RelayCode" , new DataObject(DataObject.VisibilityOptions.Member,relayJoinCode) }
                    },
                });

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation,"dtls"));

                SnakesAndLaddersMultiplayer.Instance.StartHost();

                Debug.Log("Created Lobby And Joined " + joinedLobby.Name);
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<Allocation> AllocationRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            return allocation;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return default;
        }

    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return default;
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

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new()
            {
                {"PlayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,PlayerPrefs.GetString("PlayerName"))}
            }
        };
    }

    [Command]
    private void ListPlayers()
    {
        foreach (Player player in joinedLobby.Players)
        {
            Debug.Log("Player Name Joined " + player.Data["PlayerName"].Value);
        }
    }
}
