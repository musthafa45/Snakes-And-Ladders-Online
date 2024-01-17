using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSingleUI : MonoBehaviour
{
    public static event Action<short,short> OnAnyPlayerPressedRollButton; //with Player id And Dice Face Value
    [SerializeField]
    private Button rollButton;
    [SerializeField] private short playerConnectedId;
    
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
                OnAnyPlayerPressedRollButton?.Invoke(playerConnectedId,OnDiceRolledFaceValue);
            });
            
        });
        
    }

    public short GetPlayerConnectedId() => playerConnectedId;
    public void ButtonInteractableEnabled(bool canInteractable)
    {
        rollButton.interactable = canInteractable;

        UpdateSelectedVisual(canInteractable);
    }

    public void ButtonAccessbilityCheck()
    {
        rollButton.interactable = (short)NetworkManager.Singleton.LocalClientId == playerConnectedId;
    }
    public void ButtonAccessbilityCheckRevert()
    {
        rollButton.interactable = (short)NetworkManager.Singleton.LocalClientId != playerConnectedId;
    }

    public void UpdateSelectedVisual(bool canInteractable)
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

   
}
