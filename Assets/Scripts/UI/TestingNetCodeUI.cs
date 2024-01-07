using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : NetworkBehaviour
{
    [SerializeField] private Button startHostBtn;
    [SerializeField] private Button startClientBtn;
    [SerializeField] private Transform menuUiTransform;
    private void Awake()
    {
       

        startHostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();

            HideUi();
            //WaitigForOpponentUI.Instance.ShowWaitingForOpponentUI();
            //GameStartManager.Instance.SetPlayerReady(NetworkManager.Singleton.LocalClientId);
        });

        startClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();

            HideUi();
            //WaitigForOpponentUI.Instance.ShowWaitingForOpponentUI();
            //GameStartManager.Instance.SetPlayerReady(NetworkManager.Singleton.LocalClientId);
        });
    }

    private void HideUi()
    {
        menuUiTransform.gameObject.SetActive(false);
    }
}
