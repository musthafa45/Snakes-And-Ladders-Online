using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : NetworkBehaviour
{
    //[SerializeField] private Button rollButton;
    //[SerializeField] private Image diceHolder;
    [SerializeField] private Sprite diceFace1,diceFace2, diceFace3, diceFace4, diceFace5, diceFace6;

    [SerializeField] private Button rollButtonPlayer1,rollButtonPlayer2;
    [SerializeField] private Image diceImage1,diceImage2;

    [SerializeField] private int diceFaceValue;
    [SerializeField] private int dicedPlayerId;

    [SerializeField] private PlayerLocal player1;
    [SerializeField] private PlayerLocal player2;

    [SerializeField] private TextMeshProUGUI player1NameTextMeshProGui;
    [SerializeField] private TextMeshProUGUI player2NameTextMeshProGui;

    [SerializeField] private Button menuButton;

    private void Awake()
    {
        rollButtonPlayer1.onClick.AddListener(() =>
        {
            EventManager.Instance.InvokeOnDiceRollBtnPressed(1,() =>
            {
                EnableSecondPlayerDisableFirst();

                if (GameManager.LocalInstance.GetPlayMode() == PlayMode.MultiplayerCom)
                {
                    rollButtonPlayer2.onClick.Invoke();
                }
            });

            DisableInteractionOnPlayer1();

        });

        rollButtonPlayer2.onClick.AddListener(() =>
        {
            EventManager.Instance.InvokeOnDiceRollBtnPressed(2, () =>
            {
                EnableFirstPlayerDisableSecond();
            });

            DisableInteractionOnPlayer2();
        });

        menuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void Start()
    {
        //if (PlayersInputHandler.Instance.GetFirstPlayPlayerId() == 1)
        //{
        //    //EnableFirstPlayerDisableSecond();
        //    EnableFirstPlayerDisableSecondServerRpc();

        //}
        //else if(PlayersInputHandler.Instance.GetFirstPlayPlayerId() == 2)
        //{
        //    //EnableSecondPlayerDisableFirst();
        //    EnableSecondPlayerDisableFirstServerRpc();
        //}

        player1NameTextMeshProGui.text = player1?.GetPlayerName();
        player2NameTextMeshProGui.text = player2?.GetPlayerName();
    }

    [ServerRpc]
    private void EnableSecondPlayerDisableFirstServerRpc()
    {
        EnableSecondPlayerDisableFirstClientRpc();
    }
    [ClientRpc]
    private void EnableSecondPlayerDisableFirstClientRpc()
    {
        EnableSecondPlayerDisableFirst();
    }

    [ServerRpc]
    private void EnableFirstPlayerDisableSecondServerRpc()
    {
        //EnableFirstPlayerDisableSecond();
        EnableFirstPlayerDisableSecondClientRpc(); //tells The Client Player 2
    }
    [ClientRpc]
    private void EnableFirstPlayerDisableSecondClientRpc()
    {
        EnableFirstPlayerDisableSecond();
    }

    private void EnableSecondPlayerDisableFirst()
    {
        DisableInteractionOnPlayer1();

        rollButtonPlayer2.interactable = true;
        DiceSelectorVisual diceSelectorVisualBtn2 = rollButtonPlayer2.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceSelectorVisualBtn2.DoVisual();
    }

    private void DisableInteractionOnPlayer1()
    {
        rollButtonPlayer1.interactable = false;
        DiceSelectorVisual diceSelectorVisualBtn1 = rollButtonPlayer1.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceSelectorVisualBtn1.StopVisual();
    }

    private void EnableFirstPlayerDisableSecond()
    {
        DisableInteractionOnPlayer2();

        rollButtonPlayer1.interactable = true;
        DiceSelectorVisual diceSelectorVisualBtn1 = rollButtonPlayer1.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceSelectorVisualBtn1.DoVisual();
    }

    private void DisableInteractionOnPlayer2()
    {
        rollButtonPlayer2.interactable = false;
        DiceSelectorVisual diceSelectorVisualBtn2 = rollButtonPlayer2.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceSelectorVisualBtn2.StopVisual();
    }

    private void OnEnable()
    {
        //EventManager.Instance.OnDiceRolled += EventManager_Instance_OnDiceRolled;
        //EventManager.Instance.OnPlayerWon += EventManager_Instance_OnPlayerWon;
        //EventManager.Instance.OnPlayerStartedMoving += EventManager_Instance_OnPlayerStartedMoving;
        //EventManager.Instance.OnPlayerStoppedMoving += EventManager_Instance_OnPlayerStoppedMoving;
    }

    private void EventManager_Instance_OnPlayerWon(object sender, EventArgs e)
    {
        Debug.Log("Player Win");
    }

    //private void EventManager_Instance_OnPlayerStartedMoving(object sender, EventArgs e)
    //{
    //    rollButton.interactable = false;
    //}

    //private void EventManager_Instance_OnPlayerStoppedMoving(object sender, EventArgs e)
    //{
    //   rollButton.interactable = true;
    //}
  
    private void EventManager_Instance_OnDiceRolled(object sender, EventManager.OnDicerolledArgs e)
    {
        this.diceFaceValue = e.diceFaceValue;
        this.dicedPlayerId = e.diceRolledPlayerId;

        UpdateDiceVisual(e.diceFaceValue,e.diceRolledPlayerId);
    }

    private void OnDisable()
    {
        //EventManager.Instance.OnDiceRolled -= EventManager_Instance_OnDiceRolled;
        //EventManager.Instance.OnPlayerWon -= EventManager_Instance_OnPlayerWon;
        //EventManager.Instance.OnPlayerStartedMoving -= EventManager_Instance_OnPlayerStartedMoving;
        //EventManager.Instance.OnPlayerStoppedMoving -= EventManager_Instance_OnPlayerStoppedMoving;
        //rollButton.onClick.RemoveAllListeners();

        rollButtonPlayer1.onClick.RemoveAllListeners();
        rollButtonPlayer2.onClick.RemoveAllListeners();
    }

    private void UpdateDiceVisual(int diceFaceValue,int playerId)
    {
        if(playerId == 1)
        {
            diceImage1.sprite = diceFaceValue == 1 ? diceFace1 :
                            diceFaceValue == 2 ? diceFace2 :
                            diceFaceValue == 3 ? diceFace3 :
                            diceFaceValue == 4 ? diceFace4 :
                            diceFaceValue == 5 ? diceFace5 :
                                                 diceFace6;
        }
        else if(playerId == 2)
        {
            diceImage2.sprite = diceFaceValue == 1 ? diceFace1 :
                            diceFaceValue == 2 ? diceFace2 :
                            diceFaceValue == 3 ? diceFace3 :
                            diceFaceValue == 4 ? diceFace4 :
                            diceFaceValue == 5 ? diceFace5 :
                                                 diceFace6;
        }
    }
}

