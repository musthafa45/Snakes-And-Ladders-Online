using DG.Tweening;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerLocal : NetworkBehaviour
{
    public static PlayerLocal LocalInstance { get; private set; }

    public static event Action<short> OnAnyPlayerSpawned;
    public static event Action<short> OnAnyPlayerReachedTargetTile;

    private DiceBoard diceBoard;
    [SerializeField] 
    //private float speed = 10f;
    private int standingTileId = 1;
    private bool isMovingBack = false;

    //private short playerId;
    private PathFollower pathFollower;
    private string playerName;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }

        if (!IsOwner) return;

        diceBoard = FindObjectOfType<DiceBoard>();
        pathFollower = GetComponent<PathFollower>();

        transform.position = diceBoard.GetStartPosition();

        //if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerCom && playerId == 1)
        //{
        //    playerName = "Player" + UnityEngine.Random.Range(100, 1000);
        //}

        //if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerCom && playerId == 2)
        //{
        //    playerName = "Guest" + UnityEngine.Random.Range(100, 1000);
        //}

        //if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerLocal && playerId == 1)
        //{
        //    playerName = "Player" + UnityEngine.Random.Range(100, 1000);
        //}

        //if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerLocal && playerId == 2)
        //{
        //    playerName = "Player" + UnityEngine.Random.Range(100, 1000);
        //}

        //if (NetworkManager.Singleton.LocalClientId == 0)
        //{
        //    playerId = 0;
        //}
        //else if (NetworkManager.Singleton.LocalClientId == 1)
        //{
        //    playerId = 1;
        //}

        //EventManager.Instance.OnDiceRolled += EventManager_Instance_OnDiceRolled;
        //EventManager.Instance.OnDiceRollButtonPerformed += EventManager_Instance_OnDiceRollButtonPerformed;
        //Debug.Log(" Player Id Is "+ playerId);

        pathFollower.pathCreator = null;
        pathFollower.CanMove = false;

        OnAnyPlayerSpawned?.Invoke((short)NetworkManager.Singleton.LocalClientId);

        PlayerProfileSingleUI.OnAnyPlayerPressedRollButton += PlayerProfileSingleUI_OnAnyPlayerPressedRollButton;
    }

    public override void OnNetworkDespawn()
    {
        PlayerProfileSingleUI.OnAnyPlayerPressedRollButton -= PlayerProfileSingleUI_OnAnyPlayerPressedRollButton;
    }

    private void PlayerProfileSingleUI_OnAnyPlayerPressedRollButton(short rolledPlayerId, short diceFaceValue)
    {
        if (!IsOwner) return;

        if (NetworkManager.Singleton.LocalClientId == (ulong)rolledPlayerId)
        {
            DoMovePlayer(diceFaceValue);
        }
    }

    private void DoMovePlayer(short diceFaceValue)
    {
        if (!isMovingBack) // is Moving Now 1 to 100
        {
            bool validTargetindex = standingTileId + diceFaceValue <= 100;

            if (validTargetindex)
            {
                StartCoroutine(MoveOneByOne(diceFaceValue));
            }
            else
            {
                Debug.LogWarning("Index Out Of Bound");
                //EventManager.Instance.InvokePlayerStoppedMoving();
                OnAnyPlayerReachedTargetTile?.Invoke((short)NetworkManager.Singleton.LocalClientId);
                //EventManager.Instance.InvokeMoveOutOfBound();
            }
        }
        else
        {
            bool validTargetindex = standingTileId - diceFaceValue >= 0f;

            if (validTargetindex)
            {
                StartCoroutine(MoveOneByOne(diceFaceValue));
            }
            else
            {
                Debug.LogWarning("Index Out Of Bound");
                //EventManager.Instance.InvokePlayerStoppedMoving();
                OnAnyPlayerReachedTargetTile?.Invoke((short)NetworkManager.Singleton.LocalClientId);
                //EventManager.Instance.InvokeMoveOutOfBound();
            }
        }
    }

    private IEnumerator MoveOneByOne(int targetTileIndex)
    {
        //EventManager.Instance.InvokePlayerStartedMoving();

        int targetTileId = 0;
        for (int i = 0; i < targetTileIndex; i++)
        {
            if (!isMovingBack)
            {
                targetTileId = standingTileId + 1 + i;
                Debug.Log("player " + targetTileId);
            }
            else
            {
                targetTileId = standingTileId - 1 - i;
                Debug.Log("player " + targetTileId);
            }

            yield return new WaitForSeconds(0.3f);

            Move(GetTilePositionFromId(targetTileId), MoveType.Jump,() =>
            {
                if (diceBoard.IsTileIDLadder(standingTileId))
                {
                    standingTileId = diceBoard.GetLadderEndTileId(targetTileId);
                    Invoke(nameof(MoveToLadder), 0.5f);
                    Debug.Log("Player Standing In Ladder Start");
                }

                if(diceBoard.IsTileIDSnake(standingTileId))
                {
                    standingTileId = diceBoard.GetSnakeEndTileId(targetTileId);
                    StartCoroutine(MoveToSnakeTail(targetTileId));
                    Debug.Log("Player Standing In Snake Head");
                }
               
            });
        }

        GameManager.LocalInstance.SetPlayerSuccessfullyMoved((short)NetworkManager.Singleton.LocalClientId);
        // this Is For Enable Disable Roll Dice Button
        standingTileId = targetTileId;

        if (diceBoard.GetAllTile()[^1].GetTileId() == targetTileId)
        {
            Debug.Log("100 th Tile Reached");
            isMovingBack = true;
        }

        if (isMovingBack)
        {
            // Once the player reaches back to the starting tile, reset the movement direction
            if (/*standingTileId - targetTileIndex == 0*/ standingTileId == 1)
            {
                isMovingBack = false;
                //EventManager.Instance.InvokePlayerWin();
                Debug.Log("Player Win");
            }
        }
    }

    private IEnumerator MoveToSnakeTail(int targetTileId)
    {
        yield return new WaitForSeconds(0.5f);
        Move(GetTilePositionFromId(diceBoard.GetSnakeEndTileId(targetTileId)), MoveType.Snake);
    }

    private void MoveToLadder()
    {
        Move(GetTilePositionFromId(standingTileId), MoveType.Straight);
    }

    private Vector3 GetTilePositionFromId(int targetTileId)
    {
        var targetTile = diceBoard.GetAllTile().Where(tile => tile.GetTileId() == targetTileId).FirstOrDefault();
        Vector3 target = targetTile.transform.position;
        return target;
    }

    private void Move(Vector3 targetPosition,MoveType moveType,Action OnPlayerReached = null)
    {
        if(moveType == MoveType.Jump)
        {
            transform.DOJump(targetPosition, 0.5f, 1, 0.6f).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                //if (diceBoard.GetAllTile()[^1] == targetTile)
                //{
                //    Debug.Log("100 th Tile Reached");
                //    isMovingBack = true;
                //}
                OnPlayerReached();
            });
        }
        else if(moveType == MoveType.Straight)
        {
            transform.DOMove(targetPosition, 0.5f).OnComplete(() =>
            {
                OnPlayerReached();
            });
        }
        else if(moveType == MoveType.Snake)
        {
            var pathCreator = diceBoard.GetPathCreatorFromSnakeTailId(standingTileId);
            pathFollower.pathCreator = pathCreator;
            pathFollower.CanMove = true;
            pathFollower.PathFinished = OnPlayerReached;
        }
       
    }

    public void SetPlayerName(string playerName) => this.playerName = playerName;
    public string GetPlayerName() => playerName;
    public enum MoveType
    {
        Jump,Straight,Snake
    }

}
