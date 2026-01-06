using DG.Tweening;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Player_PlayerVsCom : MonoBehaviour {
    public static Player_PlayerVsCom Instance { get; private set; }
    public event Action OnPlayerMoveDone;
    private DiceBoard diceBoard;
    private int standingTileId = 1;
    private bool isMovingBack = false;


    private PathFollower pathFollower;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        diceBoard = FindObjectOfType<DiceBoard>();
        pathFollower = GetComponent<PathFollower>();

        pathFollower.pathCreator = null;
        pathFollower.CanMove = false;

        PlayerProfileSingleUI.OnAnyPlayerPressedRollButton += PlayerProfileSingleUI_OnAnyPlayerPressedRollButton;
        CheatManager.Instance.OnPlayerPressedCheatCodeBtn += CheatManager_OnPlayerPressedCheatCodeBtn;
    }

    private void OnDestroy() {
        PlayerProfileSingleUI.OnAnyPlayerPressedRollButton -= PlayerProfileSingleUI_OnAnyPlayerPressedRollButton;
        CheatManager.Instance.OnPlayerPressedCheatCodeBtn -= CheatManager_OnPlayerPressedCheatCodeBtn;
    }


    private void CheatManager_OnPlayerPressedCheatCodeBtn(object sender, EventArgs e) {

        if (GameManager_PlayerVsCom.Instance.CurrentActivePlayerId == 0) {
            int neededMoveAmount = 100 - standingTileId;
            SetTargetTileToMove((short)neededMoveAmount);
            PlayerProfileStatsHandlerUI.Instance.GetPlayerProfileSingleUIs()[0].StopTimer();
        }
        
    }


    private void PlayerProfileSingleUI_OnAnyPlayerPressedRollButton(short rolledPlayerId, short diceFaceValue) {
        // Local Player Rolled The Dice 
        if(rolledPlayerId == 1) return;

        SetTargetTileToMove(diceFaceValue);
    }

    private void SetTargetTileToMove(short diceFaceValue) {
        if (!isMovingBack) // is Moving Now 1 to 100
        {
            bool validTargetindex = standingTileId + diceFaceValue <= 100;

            if (validTargetindex) {
                StartCoroutine(MoveOneByOne(diceFaceValue));
            }
            else {
                Debug.LogWarning("Index Out Of Bound");
                //GameManager.LocalInstance.SetPlayerReachedTarget(NetworkManager.Singleton.LocalClientId);
            }
        }
        else {
            bool validTargetindex = standingTileId - diceFaceValue >= 0f;

            if (validTargetindex) {
                StartCoroutine(MoveOneByOne(diceFaceValue));
            }
            else {
                Debug.LogWarning("Index Out Of Bound");
                //GameManager.LocalInstance.SetPlayerReachedTarget(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    private IEnumerator MoveOneByOne(int targetTileIndex) {
        int targetTileId = 0;
        for (int i = 0; i < targetTileIndex; i++) {
            if (!isMovingBack) {
                targetTileId = standingTileId + 1 + i;
                //Debug.Log("player " + targetTileId);
            }
            else {
                targetTileId = standingTileId - 1 - i;
                //Debug.Log("player " + targetTileId);
            }

            yield return new WaitForSeconds(0.3f);

            Move(GetTilePositionFromId(targetTileId), MoveType.Jump, () => {
                if (diceBoard.IsTileIDSnake(standingTileId)) {
                    standingTileId = diceBoard.GetSnakeEndTileId(targetTileId);
                    StartCoroutine(MoveToSnakeTail(targetTileId));
                    Debug.Log("Player Standing In Snake Head");
                }

                if (diceBoard.IsTileIDLadder(standingTileId)) {
                    standingTileId = diceBoard.GetLadderEndTileId(targetTileId);
                    Invoke(nameof(MoveToLadder), 0.5f);
                    Debug.Log("Player Standing In Ladder Start");
                }
            });
        }

        //GameManager.LocalInstance.SetPlayerReachedTarget(NetworkManager.Singleton.LocalClientId);
        OnPlayerMoveDone?.Invoke();

        // this Is For Enable Disable Roll Dice Button
        standingTileId = targetTileId;

        if (diceBoard.GetAllTile()[^1].GetTileId() == targetTileId) {
            Debug.Log("100 th Tile Reached");
            //isMovingBack = true;
            //Player Win One Way Mode

            Debug.Log("Player Win");
            //GameManager.LocalInstance.OnPlayerWin(NetworkManager.Singleton.LocalClientId);
            WinLossUi_PlayerVsCom.Instance.ShowWinUi();
        }

        //if (isMovingBack)
        //{
        //    if (standingTileId == 1)
        //    {
        //        isMovingBack = false;
        //        Debug.Log("Player Win");
        //        GameManager.LocalInstance.OnPlayerWin(NetworkManager.Singleton.LocalClientId);
        //    }
        //}
    }

    private IEnumerator MoveToSnakeTail(int targetTileId, Action OntailPosReached = null) {
        yield return new WaitForSeconds(0.5f);
        Move(GetTilePositionFromId(diceBoard.GetSnakeEndTileId(targetTileId)), MoveType.Snake, OntailPosReached);
    }

    private void MoveToLadder() {
        Move(GetTilePositionFromId(standingTileId), MoveType.Straight);
    }

    private Vector3 GetTilePositionFromId(int targetTileId) {
        var targetTile = diceBoard.GetAllTile().Where(tile => tile.GetTileId() == targetTileId).FirstOrDefault();
        Vector3 target = targetTile.transform.position;
        return target;
    }

    private void Move(Vector3 targetPosition, MoveType moveType, Action OnPlayerReached = null) {
        if (moveType == MoveType.Jump) {
            transform.DOJump(targetPosition, 0.5f, 1, 0.6f).SetEase(Ease.InOutBack).OnComplete(() => {
                OnPlayerReached();
            });
        }
        else if (moveType == MoveType.Straight) {
            transform.DOMove(targetPosition, 0.5f).OnComplete(() => {
                OnPlayerReached();
            });
        }
        else if (moveType == MoveType.Snake) {
            var pathCreator = diceBoard.GetPathCreatorFromSnakeTailId(standingTileId);
            pathFollower.pathCreator = pathCreator;
            pathFollower.CanMove = true;
            pathFollower.PathFinished = OnPlayerReached;
        }

    }

    public enum MoveType {
        Jump, Straight, Snake
    }

}
