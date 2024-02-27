using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSingleUI : MonoBehaviour
{
    public static event Action<short,short> OnAnyPlayerPressedRollButton; //with Player id And Dice Face Value
    [SerializeField]
    private Button rollButton;
    [SerializeField] private ulong playerConnectedId;
    [SerializeField] private TextMeshProUGUI playerNameTextMeshProGui;
    
    private DiceSelectorVisual diceSelectorVisual;
    private DiceRollAnimation diceRollAnimation;

    private void Awake()
    {
        diceSelectorVisual = rollButton.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceRollAnimation = rollButton.gameObject.GetComponentInChildren<DiceRollAnimation>();

        rollButton.onClick.AddListener(() =>
        {
            rollButton.interactable = false;
            diceRollAnimation.RollDice((OnDiceRolledFaceValue) =>
            {
                OnAnyPlayerPressedRollButton?.Invoke((short)playerConnectedId,OnDiceRolledFaceValue);
            });
            
        });
        
    }

    private void Start()
    {
        
    }

    public void ButtonInteractableEnabled(bool canInteractable)
    {
        rollButton.interactable = canInteractable;
    }

    public void SetSelectedVisual(bool canInteractable)
    {
        if (canInteractable)
        {
            diceSelectorVisual.DoVisual();
        }
        else
        {
            diceSelectorVisual.StopVisual();
        }
    }

    public void SetPlayerName(string playerName)
    {
        playerNameTextMeshProGui.text = playerName;
    }
}
