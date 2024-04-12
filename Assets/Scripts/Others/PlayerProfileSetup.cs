using System;
using UnityEngine;

public class PlayerProfileSetup : MonoBehaviour
{
    public event Action OnPlayerNameModified;
    public static PlayerProfileSetup Instance {  get; private set; }

    [SerializeField] private PlayerProfileSetupUI playerProfileSetupUI;

    private void Awake()
    {
        Instance = this;

        string playerName = PlayerPrefs.GetString("PlayerName");
        if (string.IsNullOrEmpty(playerName))
        {
            // Player Not Yet Set Name
            playerProfileSetupUI.Show();
        }
        else
        {
            playerProfileSetupUI.Hide();
            // Player Already Setupped
        }
    }

    public void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        OnPlayerNameModified?.Invoke();
    }
}
