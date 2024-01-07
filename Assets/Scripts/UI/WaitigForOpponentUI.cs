using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WaitigForOpponentUI : NetworkBehaviour
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
