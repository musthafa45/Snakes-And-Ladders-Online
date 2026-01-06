using System.Collections;
using UnityEngine;

public class GameManager_PlayerVsCom : MonoBehaviour
{
    public static GameManager_PlayerVsCom Instance { get; private set; }

    [SerializeField] private PlayerProfileSingleUI com_playerProfileSingleUI;

    public ulong CurrentActivePlayerId;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        PlayerProfileStatsHandlerUI.Instance.SetPlayerNames_PlayerVsCom();

        Invoke(nameof(InitializeFirstMove), 1f);

        Player_PlayerVsCom.Instance.OnPlayerMoveDone += Player_Instance_OnPlayerMoveDone;
        Com_PlayerVsCom.Instance.OnComputerMoveDone += Computer_Instance_OnComputerMoveDone;
    }

    private void Computer_Instance_OnComputerMoveDone() {
        PlayerProfileStatsHandlerUI.Instance.OnAnyPlayerMoveDone(1);
    }

    private void Player_Instance_OnPlayerMoveDone() {
        PlayerProfileStatsHandlerUI.Instance.OnAnyPlayerMoveDone(0);

        StartCoroutine(RandomDelayRoll());
    }
    private IEnumerator RandomDelayRoll() {
        float randomDelay = UnityEngine.Random.Range(1f, 5f);
        yield return new WaitForSeconds(randomDelay);

        com_playerProfileSingleUI.ButtonClick();
    }

    public void DoComputerMove() {
        StartCoroutine(RandomDelayRoll());
    }

    private void InitializeFirstMove() {
        ulong firstPlayer = (ulong)Random.Range(0, 2); // 0 or 1

        PlayerProfileStatsHandlerUI.Instance.SetupPlayersRandomFirstMove(firstPlayer);
    }
}
