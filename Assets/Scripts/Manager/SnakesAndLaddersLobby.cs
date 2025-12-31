using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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

    //private float lobbyHeartBeatTimer;
    private float lobbyHeatBeatTimerMax = 15;

    //private float lobbyUpdateTimer;
    private float lobbyUpdateTimerMax = 1.5f;


    private float lobbyUpdateTimer;
    private float lobbyHeartbeatTimer;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private async void Start() {
        try {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Authentication Success {AuthenticationService.Instance.PlayerId}");
            };

            if (!AuthenticationService.Instance.IsSignedIn) {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Add null check for PlayerName
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            Debug.Log($"Player Name: {playerName}");

           await InitializeLobbyType();
        }
        catch (Exception e)  // Catch all exceptions, not just LobbyServiceException
        {
            Debug.LogError($"Initialization failed: {e}");
        }
    }

    private async void OnDestroy() {
        await LeaveLobbyAsync();
        // Add these unsubscriptions
        if (lobbyType == LobbyType.SelectLobby) {
            if (SelectLobbyUi.Instance != null)
                SelectLobbyUi.Instance.OnPlayButtonClicked -= SelectLobbyUi_OnPlayButtonClicked;

            if (PrivateLobbyUi.Instance != null) {
                PrivateLobbyUi.Instance.OnPlayPrivateLobbyCreateClicked -= PrivateLobbyUi_OnPlayerClickedCreatePrivateLobbyBtn;
                PrivateLobbyUi.Instance.OnPlayPrivateLobbyJoinClicked -= PrivateLobbyUi_OnPlayPrivateLobbyJoinClicked;
            }
        }
    }
    private async void Update() {
        if (joinedLobby == null) return;

        // Lobby refresh (all players)
        lobbyUpdateTimer += Time.deltaTime;
        if (lobbyUpdateTimer >= lobbyUpdateTimerMax) {
            lobbyUpdateTimer = 0f;
            _ = RefreshLobbyAsync();
        }

        // Heartbeat (HOST ONLY)
        if (IsLobbyHost()) {
            lobbyHeartbeatTimer += Time.deltaTime;
            if (lobbyHeartbeatTimer >= lobbyHeatBeatTimerMax) {
                lobbyHeartbeatTimer = 0f;
                _ = SendHeartbeatAsync();
            }
        }
    }

    private async Task InitializeLobbyType()
    {
        if (lobbyType == LobbyType.QuickMatch)
        {
           await CreateOrJoinLobby();
        }
        else if (lobbyType == LobbyType.SelectLobby)
        {
            SelectLobbyUi.Instance.OnPlayButtonClicked += SelectLobbyUi_OnPlayButtonClicked;
            PrivateLobbyUi.Instance.OnPlayPrivateLobbyCreateClicked += PrivateLobbyUi_OnPlayerClickedCreatePrivateLobbyBtn;
            PrivateLobbyUi.Instance.OnPlayPrivateLobbyJoinClicked += PrivateLobbyUi_OnPlayPrivateLobbyJoinClicked;
        }
    }
    private async void PrivateLobbyUi_OnPlayPrivateLobbyJoinClicked(object sender, PrivateLobbyUi.OnPlayPrivateLobbyJoinClickedArgs e)
    {
       await JoinPrivateLobby(e.lobbyCode);
    }
    private async void PrivateLobbyUi_OnPlayerClickedCreatePrivateLobbyBtn(object sender, PrivateLobbyUi.OnPlayPrivateLobbyCreateClickedArgs e)
    {
        await CreatePrivateLobby(e.betData);
    }
    private async void SelectLobbyUi_OnPlayButtonClicked(object sender, SelectLobbyUi.OnPlayButtonClickedArgs e)
    {
        string gameMode = e.betData.GameMode;

       await CreateOrJoinLobby(gameMode);
    }



    private async Task CreatePrivateLobby(BetDataSO.BetData betData) {
        try {
            // Creating New Lobby With Given details
            int maxPlayer = 2;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                IsPrivate = true,
                Player = GetPlayer(),
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(betData.GameMode, maxPlayer, createLobbyOptions);

            // Create relay allocation
            Allocation allocation = await AllocationRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            // Update lobby with relay code
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject>
                {
                {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
            },
            });

            // UPDATED: Set relay server data with new API
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Start as host
            SnakesAndLaddersMultiplayer.Instance.StartHost();

            Debug.Log("Created private Lobby " + joinedLobby.Name + " And Code " + joinedLobby.LobbyCode);
            OnPlayerCreatedPrivateLobby?.Invoke(joinedLobby.LobbyCode, joinedLobby.Name);
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    private async Task JoinPrivateLobby(string lobbyCode) {
        try {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions {
                Player = GetPlayer()
            };

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            // Join relay using the code from lobby data
            JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data["RelayCode"].Value);

            // UPDATED: Set relay server data with new API (CLIENT VERSION)
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData  // IMPORTANT: Clients need this!
            );

            // Start as client
            SnakesAndLaddersMultiplayer.Instance.StartClient();

            Debug.Log("Joined Lobby By Code " + joinedLobby.Name);
            ListPlayers();
        }
        catch (LobbyServiceException e) {
            OnPrivateLobbyJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.LogException(e);
        }
    }

    //private async Task HandleLobbyUpdate()
    //{
    //    lobbyUpdateTimer += Time.deltaTime;

    //    if (lobbyUpdateTimer > lobbyUpdateTimerMax)
    //    {
    //        lobbyUpdateTimer = 0;
    //        Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
    //        joinedLobby = updatedLobby;
    //    }
    //}

    //private async Task HandleLobbyHeartBeat()
    //{
    //    lobbyHeartBeatTimer += Time.deltaTime;

    //    if (lobbyHeartBeatTimer > lobbyHeatBeatTimerMax)
    //    {
    //        lobbyHeartBeatTimer = 0;
    //        await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
    //    }
    //}

    private async Task RefreshLobbyAsync() {
        try {
            joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
        }
        catch (Exception e) {
            Debug.LogWarning($"Lobby refresh failed: {e.Message}");
        }
    }

    private async Task SendHeartbeatAsync() {
        try {
            await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
        catch (Exception e) {
            Debug.LogWarning($"Heartbeat failed: {e.Message}");
        }
    }

    private bool IsLobbyHost() {
        return joinedLobby != null &&
               joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }


    private async Task CreateOrJoinLobby() {
        try {
            QueryResponse queryResponse = await GetAvailableLobbies();

            if (queryResponse.Results.Count > 0) {
                // JOIN EXISTING LOBBY
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions {
                    Player = GetPlayer()
                };

                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id, joinLobbyByIdOptions);

                // Join relay
                JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data["RelayCode"].Value);

                // UPDATED: Set relay server data for CLIENT (with HostConnectionData)
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData  // CLIENT needs this
                );

                SnakesAndLaddersMultiplayer.Instance.StartClient();
                Debug.Log("Joined Already Created Lobby " + joinedLobby.Name);
                ListPlayers();
            }
            else {
                // CREATE NEW LOBBY
                Debug.LogWarning("Can't find lobby to join, creating new lobby");

                string lobbyName = "Quick Lobby";
                int maxPlayer = 2;
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                    Player = GetPlayer(),
                };

                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);

                // Create relay allocation
                Allocation allocation = await AllocationRelay();
                string relayJoinCode = await GetRelayJoinCode(allocation);

                // Update lobby with relay code
                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject>
                    {
                    {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                },
                });

                // UPDATED: Set relay server data for HOST (without HostConnectionData)
                if (NetworkManager.Singleton != null && NetworkManager.Singleton.TryGetComponent(out UnityTransport unityTransport)) {
                    unityTransport.SetRelayServerData(
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData
                    );
                }

                SnakesAndLaddersMultiplayer.Instance.StartHost();
                Debug.Log("Created Lobby And Joined " + joinedLobby.Name);
            }
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async Task DeleteLobbyAsync()
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



    public async Task LeaveLobbyAsync() {
        try {
            if (joinedLobby == null || NetworkManager.Singleton == null) return;

            // HOST leaves → delete lobby
            if (NetworkManager.Singleton.IsHost) {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                Debug.Log("Host deleted the lobby");
            }
            else {
                // CLIENT leaves -> remove self
                await LobbyService.Instance.RemovePlayerAsync(
                    joinedLobby.Id,
                    AuthenticationService.Instance.PlayerId
                );
                Debug.Log("Client left the lobby");
            }

            joinedLobby = null;
        }
        catch (LobbyServiceException e) {
            Debug.LogWarning($"LeaveLobby failed: {e.Message}");
        }
    }



    private async Task CreateOrJoinLobby(string gameMode) {
        try {
            QueryResponse queryResponse = await GetSelectedModeLobbies(gameMode);
            Debug.Log("Selected Mode Lobbies " + gameMode + " Count: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results) {
                Debug.Log("Lobby " + lobby.Name);
            }

            if (queryResponse.Results.Count > 0) {
                // JOIN EXISTING LOBBY
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions {
                    Player = GetPlayer(),
                };

                // FIXED: Use LobbyService.Instance instead of Lobbies.Instance
                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id, joinLobbyByIdOptions);

                // Join relay
                JoinAllocation joinAllocation = await JoinRelay(joinedLobby.Data["RelayCode"].Value);

                // UPDATED: Set relay server data for CLIENT (with HostConnectionData)
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData  // CLIENT needs this
                );

                SnakesAndLaddersMultiplayer.Instance.StartClient();
                Debug.Log("Joined Already Created Lobby " + joinedLobby.Name);
                ListPlayers();
            }
            else {
                // CREATE NEW LOBBY
                Debug.LogWarning("Can't find lobby to join, creating new lobby");

                string lobbyName = gameMode;
                int maxPlayer = 2;
                CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                    {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)},
                }
                };

                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);

                // Create relay allocation
                Allocation allocation = await AllocationRelay();
                string relayJoinCode = await GetRelayJoinCode(allocation);

                // Update lobby with relay code
                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject>
                    {
                    {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                },
                });

                // UPDATED: Set relay server data for HOST (without HostConnectionData)
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );

                SnakesAndLaddersMultiplayer.Instance.StartHost();
                Debug.Log("Created Lobby And Joined " + joinedLobby.Name);
            }
        }
        catch (LobbyServiceException e) {
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
        catch (RelayServiceException e)
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
        catch (RelayServiceException e)
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

    public async Task<QueryResponse> GetAvailableLobbies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Count = 10,
                Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            },
                Order = new List<QueryOrder>
                {
                new QueryOrder(asc: false, field: QueryOrder.FieldOptions.Created)
            }
            };

            // FIXED: Use LobbyService.Instance instead of Lobbies.Instance
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            return queryResponse;
        }
        catch (LobbyServiceException e) {
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

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
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

    public LobbyType GetLobbyType()
    {
        return lobbyType;
    }


    private void OnApplicationQuit() {
        _ = LeaveLobbyAsync();
    }
}
