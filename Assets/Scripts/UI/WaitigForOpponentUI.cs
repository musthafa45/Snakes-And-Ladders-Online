using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaitigForOpponentUI : MonoBehaviour
{
    public static WaitigForOpponentUI Instance {  get; private set; }

    [SerializeField] private Transform waitingForOpponentUiParent;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        HideWaitingForOpponentUI();

        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged -= GameManager_OnLocalPlayerReadyChanged;
    }

    private void GameManager_OnLocalPlayerReadyChanged(bool isPlayerReady)
    {
        if (isPlayerReady)
        {
            ShowWaitingForOpponentUI();
        }
    }

    public void ShowWaitingForOpponentUI()
    {
        waitingForOpponentUiParent.gameObject.SetActive(true);
    }

    public void HideWaitingForOpponentUI()
    {
        waitingForOpponentUiParent.gameObject.SetActive(false);
    }
}
