using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance {  get; private set; }

    [SerializeField] private Button menuButton;
    private void Awake()
    {
        Instance = this;

        menuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SnakesAndLaddersLobby.Instance.LeaveLobby();
            Loader.LoadScene(Loader.Scene.MainMenu);
        });
    }

    public void ShowGameFinishedUi(ulong winLocalClientId)
    {
        if(winLocalClientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("You Won");
        }
        else
        {
            Debug.Log("You Loss");
        }
    }
}

