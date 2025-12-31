using Unity.Netcode;
using UnityEngine;

public class HostDisconnectListener : MonoBehaviour {
    private void Start() {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong clientId) {
        // Client-side: host disconnected
        if (clientId == NetworkManager.ServerClientId) {
            Debug.Log("HOST DISCONNECTED");

            // TODO:
            // Show win UI
            // Disable input
            // Return to menu
        }
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}
