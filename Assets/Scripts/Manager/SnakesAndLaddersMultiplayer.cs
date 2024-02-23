using System;
using Unity.Netcode;
using UnityEngine;

public class SnakesAndLaddersMultiplayer : NetworkBehaviour
{
    public static SnakesAndLaddersMultiplayer Instance { get; private set; }

    public event EventHandler OnPlayerDataNetworkListChanged;
    private NetworkList<PlayerData> playerDataNetworkList;



    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        if(playerDataNetworkList.Count == 2)
        {
            Debug.Log("Two Players Connected Start Match");
            NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;

        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData 
        { 
            clientId = clientId 
        });  
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
