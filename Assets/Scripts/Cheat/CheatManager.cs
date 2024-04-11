using System;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }

    public event EventHandler OnPlayerPressedCheatCodeBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            OnPlayerPressedCheatCodeBtn?.Invoke(this, EventArgs.Empty);
        }
    }
}
