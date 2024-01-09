using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    private DiceBoard diceBoard;
    //[SerializeField] private float speed = 10f;
    private Tile standingTile;
    private bool isMovingBack = false;
    private List<Tile> tiles = new(100);
    private Coroutine moveCoroutine;
    [SerializeField] private PlayerTurnController turnController;

    public override void OnNetworkSpawn()
    {
        transform.position = diceBoard.GetAllTile()[0].transform.position;

        //if(IsHost)
        //{
        //    turnController.SetPlayerTurn();
        //}
        //else if(IsClient)
        //{

        //}
    }
    private void Awake()
    {
        diceBoard = FindObjectOfType<DiceBoard>();
    }
    private void OnEnable()
    {
        EventManager.Instance.OnDiceRolled += EventManager_Instance_OnDiceRolled;
    }
    private void OnDisable()
    {
        EventManager.Instance.OnDiceRolled -= EventManager_Instance_OnDiceRolled;
    }
    private void Start()
    {
        tiles = diceBoard.GetAllTile();
        standingTile = tiles[0];
    }
    private void EventManager_Instance_OnDiceRolled(object sender, EventManager.OnDicerolledArgs e)
    {

        if (!IsOwner) return;

        if (moveCoroutine != null) return;

        if (!isMovingBack)
        {
            bool validTargetindex = tiles.IndexOf(this.standingTile) + e.diceFaceValue < 100f;

            if (validTargetindex)
            {
                int standingTileIndex = tiles.IndexOf(this.standingTile);
                moveCoroutine = StartCoroutine(MoveOneByOne(standingTileIndex, e.diceFaceValue));
            }
            else
            {
                Debug.LogWarning("Index Out Of Bound");
                EventManager.Instance.InvokeMoveOutOfBound();
            }
        }
        else
        {
            bool validTargetindex = tiles.IndexOf(this.standingTile) - e.diceFaceValue >= 0f;

            if (validTargetindex)
            {
                int standingTileIndex = tiles.IndexOf(this.standingTile);
                StartCoroutine(MoveOneByOne(standingTileIndex, e.diceFaceValue));
            }
            else
            {
                Debug.LogWarning("Index Out Of Bound");
                EventManager.Instance.InvokeMoveOutOfBound();
            }
        }


    }

    private IEnumerator MoveOneByOne(int standingTileIndex, int targetTileIndex)
    {
        EventManager.Instance.InvokePlayerStartedMoving();

        for (int i = 0; i < targetTileIndex; i++)
        {
            Tile tileToMove;
            if (!isMovingBack)
            {
                tileToMove = tiles[standingTileIndex + 1 + i];
            }
            else
            {
                tileToMove = tiles[standingTileIndex - 1 - i];
            }

            yield return new WaitForSeconds(0.3f);
            Move(tileToMove);
          
        }

        EventManager.Instance.InvokePlayerStoppedMoving();

        if (isMovingBack)
        {
            // Once the player reaches back to the starting tile, reset the movement direction
            if (standingTileIndex - targetTileIndex <= 0)
            {
                isMovingBack = false;
                EventManager.Instance.InvokePlayerWin();
            }
        }
    }

    private void Move(Tile tileToMove)
    {
        Vector3 target = tileToMove.transform.position;

        transform.DOJump(target, 0.5f, 1, 0.6f).SetEase(Ease.InOutBack).OnComplete(() =>
        {
            standingTile = tileToMove;

            if (tiles[^1] == standingTile)
            {
                Debug.Log("100 th Tile Reached");
                isMovingBack = true;
            }

            moveCoroutine = null;
        });
    }

}