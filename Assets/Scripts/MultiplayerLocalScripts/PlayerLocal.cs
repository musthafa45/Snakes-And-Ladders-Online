using DG.Tweening;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerLocal : NetworkBehaviour
{
    private DiceBoard diceBoard;
    [SerializeField] 
    //private float speed = 10f;
    private int standingTileId = 1;
    private bool isMovingBack = false;

    [SerializeField] private int playerId;

    private Action OnPlayerReachedTargetTile;
    private PathFollower pathFollower;
    private string playerName;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        base.OnNetworkSpawn();

        diceBoard = FindObjectOfType<DiceBoard>();
        pathFollower = GetComponent<PathFollower>();

        transform.position = diceBoard.GetStartPosition();

        if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerCom && playerId == 1)
        {
            playerName = "Player" + UnityEngine.Random.Range(100, 1000);
        }

        if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerCom && playerId == 2)
        {
            playerName = "Guest" + UnityEngine.Random.Range(100, 1000);
        }

        if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerLocal && playerId == 1)
        {
            playerName = "Player" + UnityEngine.Random.Range(100, 1000);
        }

        if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerLocal && playerId == 2)
        {
            playerName = "Player" + UnityEngine.Random.Range(100, 1000);
        }

        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            playerId = 1;
        }
        else if(NetworkManager.Singleton.LocalClientId == 1)
        {
            playerId = 2;
        }

        EventManager.Instance.OnDiceRolled += EventManager_Instance_OnDiceRolled;
        EventManager.Instance.OnDiceRollButtonPerformed += EventManager_Instance_OnDiceRollButtonPerformed;

        pathFollower.pathCreator = null;
        pathFollower.CanMove = false;
    }
    private void Awake()
    {
        //if (GameManager.Instance.GetPlayMode() == PlayMode.MultiplayerOnline && !IsHost) Destroy(this.gameObject);

       
    }
    private void OnEnable()
    {
        
    }

    private void EventManager_Instance_OnDiceRollButtonPerformed(object sender, EventManager.OnDiceRollButtonPerformedArgs e)
    {
        if (!IsOwner) return;

        if (e.diceRolledPlayerId == playerId)
        {
            this.OnPlayerReachedTargetTile = e.OnPlayerMovedSuccessfully; // we Need Moved After Callback For Button Interactable 
        }

    }

    private void OnDisable()
    {
        EventManager.Instance.OnDiceRolled -= EventManager_Instance_OnDiceRolled;
    }

    private void EventManager_Instance_OnDiceRolled(object sender, EventManager.OnDicerolledArgs e)
    {
        if (!IsOwner) return;

        if (e.diceRolledPlayerId != this.playerId) return;

        if (!isMovingBack) // is Moving Now 1 to 100
        {
            bool validTargetindex = standingTileId + e.diceFaceValue <= 100;

            if (validTargetindex)
            {
                StartCoroutine(MoveOneByOne(e.diceFaceValue));
            }
            else
            {
                Debug.LogWarning("Index Out Of Bound");
                EventManager.Instance.InvokePlayerStoppedMoving();
                OnPlayerReachedTargetTile();
                EventManager.Instance.InvokeMoveOutOfBound();
            }
        }
        else
        {
            bool validTargetindex = standingTileId - e.diceFaceValue >= 0f;

            if (validTargetindex)
            {
                StartCoroutine(MoveOneByOne(e.diceFaceValue));
            }
            else
            {
                Debug.LogWarning("Index Out Of Bound");
                EventManager.Instance.InvokePlayerStoppedMoving();
                OnPlayerReachedTargetTile();
                EventManager.Instance.InvokeMoveOutOfBound();
            }
        }


    }

    private IEnumerator MoveOneByOne(int targetTileIndex)
    {
        EventManager.Instance.InvokePlayerStartedMoving();

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

        EventManager.Instance.InvokePlayerStoppedMoving();
        OnPlayerReachedTargetTile?.Invoke(); // this Is For Enable Disable Roll Dice Button
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
                EventManager.Instance.InvokePlayerWin();
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
