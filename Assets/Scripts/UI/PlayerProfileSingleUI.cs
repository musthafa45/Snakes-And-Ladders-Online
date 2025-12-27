using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DiceData {
    public Sprite diceFaceSprite;
    public short diceFaceValue;
}

public class PlayerProfileSingleUI : MonoBehaviour
{
    public static event Action<short,short> OnAnyPlayerPressedRollButton; //with Player id And Dice Face Value
    [SerializeField]
    private Button rollButton;
    [SerializeField] private ulong playerConnectedId;
    [SerializeField] private TextMeshProUGUI playerNameTextMeshProGui;
    
    private DiceSelectorVisual diceSelectorVisual;
    private DiceRollAnimation diceRollAnimation;

    private short diceFaceValue;

    private void Awake()
    {
        diceSelectorVisual = rollButton.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceRollAnimation = rollButton.gameObject.GetComponentInChildren<DiceRollAnimation>();

        rollButton.onClick.AddListener(() =>
        {
            rollButton.interactable = false;

            diceFaceValue = GetRandomDiceData().diceFaceValue; // Get Random Dice Face Value

            // Send rolled value to server
            GameManager.LocalInstance.SendDiceValueToOpponentServerRpc(diceFaceValue); // for Opponent Animation

            diceRollAnimation.RollDice(diceFaceValue,()=> { // Play Local Dice Animation
                OnAnyPlayerPressedRollButton?.Invoke((short)playerConnectedId, diceFaceValue); // for Player Movement
            }); 

        });
        
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

    private DiceData GetRandomDiceData() {
        int randomIndex = UnityEngine.Random.Range(0, diceRollAnimation.GetDiceDataList().Count);
        return diceRollAnimation.GetDiceDataList()[randomIndex];
    }

    public DiceRollAnimation GetDiceRollAnimation() {
        return diceRollAnimation;
    }

}
