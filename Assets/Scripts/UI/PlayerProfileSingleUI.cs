using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
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

    [SerializeField] private Button rollButton;
    [SerializeField] private ulong playerConnectedId;
    [SerializeField] private TextMeshProUGUI playerNameTextMeshProGui;

    [SerializeField] private TextMeshProUGUI timerTextMeshProGui;
    [SerializeField] private Image timerBg;
    [SerializeField] private Image timerFillImage;
    [SerializeField] private int timerMax = 30;
    [SerializeField] private int timer = 0;
    [SerializeField] private bool isOpponentDice = false;
    private const int urgentThreshold = 15;

    private DiceSelectorVisual diceSelectorVisual;
    private DiceRollAnimation diceRollAnimation;

    private short diceFaceValue;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        diceSelectorVisual = rollButton.gameObject.GetComponentInChildren<DiceSelectorVisual>();
        diceRollAnimation = rollButton.gameObject.GetComponentInChildren<DiceRollAnimation>();

        rollButton.onClick.AddListener(() =>
        {
            rollButton.interactable = false;

            diceFaceValue = GetRandomDiceData().diceFaceValue; // Get Random Dice Face Value

            if (GameManager.LocalInstance != null && NetworkManager.Singleton != null) {
                // Send rolled value to server
                GameManager.LocalInstance.SendDiceValueToOpponentServerRpc(diceFaceValue); // for Opponent Animation
            }
           
            diceRollAnimation.RollDice(diceFaceValue,()=> { // Play Local Dice Animation
                OnAnyPlayerPressedRollButton?.Invoke((short)playerConnectedId, diceFaceValue); // for Player Movement
            });

            TimerOff();

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
            TimerOn();
        }
        else
        {
            diceSelectorVisual.StopVisual();
            TimerOff();
        }
    }

    private void TimerOn() {
        TimerOff(); // safety

        timer = timerMax;
        timerFillImage.fillAmount = 1f;

        timerFillImage.gameObject.SetActive(true);
        timerBg.gameObject.SetActive(true);
        timerTextMeshProGui.gameObject.SetActive(true);
        timerTextMeshProGui.text = timer.ToString();

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private void TimerOff() {
        if (timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        timerFillImage.gameObject.SetActive(false);
        timerBg.gameObject.SetActive(false);
        timerTextMeshProGui.gameObject.SetActive(false);
    }


    private IEnumerator TimerCoroutine() {
        float timeLeft = timerMax;
        int lastSecondShown = Mathf.CeilToInt(timeLeft);

        while (timeLeft > 0f) {
            timeLeft -= Time.deltaTime;

            // Fill Image
            timerFillImage.fillAmount = timeLeft / timerMax;

            // Text update (only when second changes)
            int currentSecond = Mathf.CeilToInt(timeLeft);
            if (currentSecond != lastSecondShown) {
                lastSecondShown = currentSecond;
                timerTextMeshProGui.text = currentSecond.ToString();
            }

            if (currentSecond <= urgentThreshold) {
                timerTextMeshProGui.color = Color.red;
            }
            else {
                timerTextMeshProGui.color = Color.white;
            }


            yield return null;
        }

        // Time Over
        timerFillImage.fillAmount = 0f;
        timerTextMeshProGui.text = "0";
        rollButton.interactable = false;

        RollDiceTimeOut(); // optional
    }

    private void RollDiceTimeOut() {

        Debug.Log("Roll Timer Finished");

        if(!isOpponentDice && NetworkManager.Singleton != null) {
            GameManager.LocalInstance.RollTimeoutServerRpc();
        }
        
        if(NetworkManager.Singleton == null) {
            ButtonClick();
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

    public void ButtonClick() {
        rollButton.onClick.Invoke();
    }

    public void StopTimer() {
        TimerOff();
    }
}
