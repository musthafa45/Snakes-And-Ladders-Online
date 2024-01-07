using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }   

    public event EventHandler<OnDiceRollButtonPerformedArgs> OnDiceRollButtonPerformed;
    public event EventHandler<OnDicerolledArgs> OnDiceRolled;
    public class OnDicerolledArgs : EventArgs
    {
        public int diceFaceValue;
        public int diceRolledPlayerId;
    }
    public class OnDiceRollButtonPerformedArgs : EventArgs
    {
        public int diceRolledPlayerId;
        public Action OnPlayerMovedSuccessfully;
    }
    [HideInInspector]public int diceRolledPlayerId;
    public event EventHandler OnPlayerStartedMoving;
    public event EventHandler OnPlayerStoppedMoving;

    public event EventHandler OnPlayerWon;
    public event EventHandler OnPlayerMoveOutOfBound;
    private void Awake()
    {
        Instance = this;
    }

    public void InvokeOnDiceRollBtnPressed(int playerId, Action OnPlayerMovedSuccessfully)
    {
        OnDiceRollButtonPerformed?.Invoke(this, new OnDiceRollButtonPerformedArgs
        {
            diceRolledPlayerId = playerId,
            OnPlayerMovedSuccessfully = OnPlayerMovedSuccessfully
        });
    }

    public void InvokeOnDiceRolled(int diceFaceValue,int rolledPlayerId)
    {
        OnDiceRolled?.Invoke(this, new OnDicerolledArgs
        {
            diceFaceValue = diceFaceValue,
            diceRolledPlayerId = rolledPlayerId
        });
    }
    public void InvokePlayerStartedMoving()
    {
        OnPlayerStartedMoving?.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerStoppedMoving()
    {
        OnPlayerStoppedMoving?.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerWin()
    {
        OnPlayerWon?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeMoveOutOfBound()
    {
        OnPlayerMoveOutOfBound?.Invoke(this, EventArgs.Empty);
    }
}
