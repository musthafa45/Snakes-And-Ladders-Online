using System;
using Unity.Netcode;
using UnityEngine;

public class SnakesAndLaddersMultiplayer : NetworkBehaviour
{
    public static SnakesAndLaddersMultiplayer Instance { get; private set; }

    public event EventHandler<OnPlayerDisConnectedEventArgs> OnRemotePlayerDisconnected;
    public event EventHandler<OnPlayerDisConnectedEventArgs> OnServerDisconnected;

    public class OnPlayerDisConnectedEventArgs : EventArgs {
        public ulong disconnectedClientId; 
    }

    public event EventHandler OnPlayerDataNetworkListChanged;
    private NetworkList<PlayerData> playerDataNetworkList;


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {

        bool isLocal = clientId == NetworkManager.Singleton.LocalClientId;
        bool isServer = NetworkManager.Singleton.IsServer;

        // 2️⃣ CLIENT SIDE: host left → connection lost
        if (!isServer && isLocal) {
            Debug.Log("Host disconnected (client lost server)");
            OnServerDisconnected?.Invoke(this, new OnPlayerDisConnectedEventArgs { disconnectedClientId = clientId});
            return;
        }

        // 3️⃣ SERVER SIDE: remote client left → server wins
        if (isServer && !isLocal) {
            Debug.Log("Remote client disconnected");
            OnRemotePlayerDisconnected?.Invoke(this, new OnPlayerDisConnectedEventArgs { disconnectedClientId = clientId });
            return;
        }

    }


    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);

        if (playerDataNetworkList.Count == 2)
        {
            Debug.Log("Two Players Connected Start Match");
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        string playerName = PlayerPrefs.GetString("PlayerName").ToString();
        ModifyNetWorkDataListRequestServerRpc(clientId, playerName);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    public override void OnDestroy() {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }


    [ServerRpc(RequireOwnership = false)]
    private void ModifyNetWorkDataListRequestServerRpc(ulong clientId, string playerName) {
        playerDataNetworkList.Add(new PlayerData {
            clientId = clientId,
            clientName = playerName
        });
    }


    //public NetworkList<PlayerData> GetPlayerDataNetWorkList()
    //{
    //    return playerDataNetworkList;
    //}
}
