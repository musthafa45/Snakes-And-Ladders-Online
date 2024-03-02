using System;
using Unity.Netcode;
using UnityEngine;

public class SnakesAndLaddersMultiplayer : NetworkBehaviour
{
    public static SnakesAndLaddersMultiplayer Instance { get; private set; }

    public event EventHandler<OnClientDisconnectedArgs> OnClientDisconnected;
    public class OnClientDisconnectedArgs : EventArgs
    {
        public ulong clientId;
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

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //Debug.Log("Client DisConnected "+ clientId);

        OnClientDisconnected?.Invoke(this, new OnClientDisconnectedArgs
        {
            clientId = clientId
        });
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

    //public override void OnDestroy()
    //{
    //    NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    //}
    

    [ServerRpc(RequireOwnership = false)]
    private void ModifyNetWorkDataListRequestServerRpc(ulong clientId, string playerName)
    {
        ModifyNetWorkDataListRequestClientRpc(clientId, playerName);
    }

    [ClientRpc]
    private void ModifyNetWorkDataListRequestClientRpc(ulong clientId, string playerName)
    {
        if (IsHost)
        {
            playerDataNetworkList.Add(new PlayerData
            {
                clientId = clientId,
                clientName = playerName
            });
        }
    }

    public NetworkList<PlayerData> GetPlayerDataNetWorkList()
    {
        return playerDataNetworkList;
    }
}
