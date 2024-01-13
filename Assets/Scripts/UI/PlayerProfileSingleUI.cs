using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileSingleUI : MonoBehaviour
{
    public static event Action<short> OnAnyPlayerPressedRollButton; //with Player id
    [SerializeField]
    private Button rollButton;
    [SerializeField] private short playerConnectedId;
    
    private DiceSelectorVisual diceSelectorVisual;

    private void Awake()
    {
        diceSelectorVisual = rollButton.gameObject.GetComponentInChildren<DiceSelectorVisual>();

        rollButton.onClick.AddListener(() =>
        {
            OnAnyPlayerPressedRollButton?.Invoke(playerConnectedId);
        });
        
    }

    public short GetPlayerConnectedId() => playerConnectedId;
    public void ButtonInteractableEnabled(bool canInteractable)
    {
        rollButton.interactable = canInteractable;

        UpdateVisual(canInteractable);
    }

    private void UpdateVisual(bool canInteractable)
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
