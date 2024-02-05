using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] private Button muliplayerOnlineButton;
    [SerializeField] private Button multiplayerLocalButton;
    [SerializeField] private Button multiplayerComputerButton;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();

        muliplayerOnlineButton.onClick.AddListener(() =>
        {
            //SceneManager.LoadScene("MultiplayerOnline");
            PlayerPrefs.SetInt("PlayMode", 3);
            SceneManager.LoadScene("MultiplayerLocalAndCom");
        });

        multiplayerLocalButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("PlayMode", 1);
            SceneManager.LoadScene("MultiplayerLocalAndCom");
        });

        multiplayerComputerButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("PlayMode", 2);
            SceneManager.LoadScene("MultiplayerLocalAndCom");
        });

    }
}
