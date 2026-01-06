using UnityEngine;

public class GameManager_PassAndPlay : MonoBehaviour
{
    public static GameManager_PassAndPlay Instance { get; private set; }
    private string playerName1,playerName2;
    public ulong CurrentActivePlayerId;

    private void Awake() {
        Instance = this;
    }
    private void Start() {
        Player_PassAndPlay.OnAnyPlayerMoveDone += Player_passAndPlay_Instance_OnPlayerMoveDone;
    }

    private void OnDestroy() {
        Player_PassAndPlay.OnAnyPlayerMoveDone -= Player_passAndPlay_Instance_OnPlayerMoveDone;
    }

    private void Player_passAndPlay_Instance_OnPlayerMoveDone(short playerId) {
        PlayerProfileStatsHandlerUI.Instance.OnAnyPlayerMoveDone((ulong)playerId);
    }

    private void InitializeFirstMove() {
        ulong firstPlayer = (ulong)Random.Range(0, 2); // 0 or 1
        PlayerProfileStatsHandlerUI.Instance.SetupPlayersRandomFirstMove(firstPlayer);
    }

    public void StartGame() {
        Invoke(nameof(InitializeFirstMove), 1f);
    }

    public void SetPlayerNames(string player1,string player2) {
        playerName1 = player1;
        playerName2 = player2;
    }

    public string GetPlayerName1() { 
        return playerName1; 
    }

    public string GetPlayerName2() { 
        return playerName2; 
    }

}
