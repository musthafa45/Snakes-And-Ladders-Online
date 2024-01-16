using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSingleUI : NetworkBehaviour
{
    public static event Action<short,short> OnAnyPlayerPressedRollButton; //with Player id And Dice Face Value
    [field: SerializeField] public short PlayerConnectedId {  get; private set; }

    [SerializeField]
    private Button rollButton;
    
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
                OnAnyPlayerPressedRollButton?.Invoke(PlayerConnectedId,OnDiceRolledFaceValue);
            });
            
        });
        
    }
    public void ButtonInteractableEnabled(bool canInteractable)
    {
        rollButton.interactable = canInteractable;

        UpdateSelectedVisual(canInteractable);
    }

    public void ButtonAccessbilityCheck()
    {
        rollButton.interactable = (short)NetworkManager.Singleton.LocalClientId == PlayerConnectedId;
    }
    public void ButtonAccessbilityCheckRevert()
    {
        rollButton.interactable = (short)NetworkManager.Singleton.LocalClientId != PlayerConnectedId;
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
