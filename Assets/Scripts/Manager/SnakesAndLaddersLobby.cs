using QFSW.QC;
using System;
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

    public event Action<string,string> OnPlayerCreatedPrivateLobby; // Lobby Code, Lobby Name
    public event EventHandler OnPrivateLobbyJoinFailed;

    public enum LobbyType
    {
        QuickMatch,SelectLobby
    }

    [SerializeField] private LobbyType lobbyType = LobbyType.QuickMatch;

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
        try
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Authendication Success {AuthenticationService.Instance.PlayerId}");
            };

            if(!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
           
            Debug.Log(PlayerPrefs.GetString("PlayerName"));

            InitializeLobbyType();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private async void Update()
    {
        if(joinedLobby != null)
        {
            await HandleLobbyHeartBeat();
            await HandleLobbyUpdate();
        }
    }
    private void InitializeLobbyType()
    {
        if (lobbyType == LobbyType.QuickMatch)
        {
            CreateOrJoinLobby();
        }
        else if (lobbyType == LobbyType.SelectLobby)
        {
            SelectLobbyUi.Instance.OnPlayButtonClicked += SelectLobbyUi_OnPlayButtonClicked;
            PrivateLobbyUi.Instance.OnPlayPrivateLobbyCreateClicked += PrivateLobbyUi_OnPlayerClickedCreatePrivateLobbyBtn;
            PrivateLobbyUi.Instance.OnPlayPrivateLobbyJoinClicked += PrivateLobbyUi_OnPlayPrivateLobbyJoinClicked;
        }
    }
    private void PrivateLobbyUi_OnPlayPrivateLobbyJoinClicked(object sender, PrivateLobbyUi.OnPlayPrivateLobbyJoinClickedArgs e)
    {
        JoinPrivateLobby(e.lobbyCode);
    }
    private void PrivateLobbyUi_OnPlayerClickedCreatePrivateLobbyBtn(object sender, PrivateLobbyUi.OnPlayPrivateLobbyCreateClickedArgs e)
    {
        CreatePrivateLobby(e.betData);
    }
    private void SelectLobbyUi_OnPlayButtonClicked(object sender, SelectLobbyUi.OnPlayButtonClickedArgs e)
    {
        string gameMode = e.betData.GameMode;

        CreateOrJoinLobby(gameMode);
    }



    private async void CreatePrivateLobby(LobbyBetSelect.BetData betData)
    {
        try
        {
            // Creating New Lobby With Given details
            int maxPlayer = 2;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,

                Player = GetPlayer(),
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(betData.GameMode, maxPlayer, createLobbyOptions);

            Allocation allocation = await AllocationRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                {"RelayCode" , new DataObject(DataObject.VisibilityOptions.Member,relayJoinCode) }
            },
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            SnakesAndLaddersMultiplayer.Instance.StartHost();

            Debug.Log("Created private Lobby " + joinedLobby.Name + " And Code " + joinedLobby.LobbyCode);
            OnPlayerCreatedPrivateLobby?.Invoke(joinedLobby.LobbyCode,joinedLobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }
    private async void JoinPrivateLobby(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions();
            joinLobbyByCodeOptions.Player = GetPlayer();

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode,joinLobbyByCodeOptions);

            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data["RelayCode"].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            SnakesAndLaddersMultiplayer.Instance.StartClient();

            Debug.Log("Joined Lobby By Code " + joinedLobby.Name);

            ListPlayers();
        }
        catch (LobbyServiceException e)
        {
            OnPrivateLobbyJoinFailed?.Invoke(this, EventArgs.Empty);

            Debug.LogException(e);
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
                JoinLobbyByIdOptions joinLobbyByIdOptions = new();
                joinLobbyByIdOptions.Player = GetPlayer();
                
                joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id,joinLobbyByIdOptions);

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

                if(NetworkManager.Singleton != null && NetworkManager.Singleton.TryGetComponent(out UnityTransport unityTransport))
                {
                    unityTransport.SetRelayServerData(new RelayServerData(allocation, "dtls"));
                }

                SnakesAndLaddersMultiplayer.Instance.StartHost();

                Debug.Log("Created Lobby And Joined " + joinedLobby.Name);
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            if(joinedLobby != null)
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void LeaveLobby()
    {
        try
        {
            if (joinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void CreateOrJoinLobby(string gameMode)
    {
        try
        {
            QueryResponse queryResponse = await GetSelectedModeLobbies(gameMode);

            Debug.Log("Selected Mode Lobbies "+ gameMode + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Lobby " + lobby.Name);
            }

            if (queryResponse.Results.Count > 0)
            {
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
                {
                    Player = GetPlayer(),
                };

                joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id,joinLobbyByIdOptions);                

                JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data["RelayCode"].Value);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

                SnakesAndLaddersMultiplayer.Instance.StartClient();

                Debug.Log("Joined Already Created Lobby " + joinedLobby.Name);

                ListPlayers();
            }
            else
            {
                // Creating New Lobby With Given details
                Debug.LogWarning("Cant Found One Lobby To Join & try Creating Lobby");

                string lobbyName = gameMode;
                int maxPlayer = 2;

                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
                {
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                        {"GameMode" , new DataObject(DataObject.VisibilityOptions.Public,gameMode) },
                    }
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

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

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

    public async Task<QueryResponse> GetSelectedModeLobbies(string gameMode)
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();

            queryLobbiesOptions.Count = 10;

            queryLobbiesOptions.Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.Name , gameMode , QueryFilter.OpOptions.EQ),
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots , "0" , QueryFilter.OpOptions.GT)
            };

            queryLobbiesOptions.Order = new List<QueryOrder>
            {
                    new QueryOrder(false , QueryOrder.FieldOptions.Created)
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
