using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if(NetworkManager.Singleton  != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if(SnakesAndLaddersLobby.Instance != null)
        {
            Destroy(SnakesAndLaddersLobby.Instance.gameObject);
        }

        if(SnakesAndLaddersMultiplayer.Instance != null)
        {
            Destroy(SnakesAndLaddersMultiplayer.Instance.gameObject);
        }
    }
}
