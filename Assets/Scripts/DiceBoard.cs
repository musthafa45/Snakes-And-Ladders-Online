using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceBoard : MonoBehaviour
{
    [SerializeField] private int boardTileRowCount = 10;
    [SerializeField] private int boardTileColumnCount = 10;

    [SerializeField] private Transform boardStartPoint;
    [SerializeField] private Transform tilePrefab;

    [SerializeField] private List<LadderData> laddersData;
    [SerializeField] private List<SnakesData> snakesData;

    private List<Tile> tiles = new(100);
    private void Awake()
    {
        SpawnTiles();
    }

    private void SpawnTiles()
    {
        int tileId = 1;
        for (int i = 0; i < boardTileRowCount; i++)
        {
            for (int j = 0; j < boardTileColumnCount; j++)
            {
                int currentIndex = i % 2 == 0 ? j : (boardTileColumnCount - 1 - j); // for Achive "S" Pattern

                var obj = Instantiate(tilePrefab, transform);
                obj.transform.position = new Vector3(currentIndex / 2f, i / 2f, 0) + boardStartPoint.position;

                if (obj.TryGetComponent<Tile>(out var tile))
                {
                    tiles.Add(tile);
                    tile.SetTileId(tileId);
                }
                else
                {
                    Debug.LogWarning("Tile component not found on the instantiated object!");
                }

                tileId++;
            }
        }
    }

    public List<Tile> GetAllTile()
    {
        return tiles;
    }

    public bool IsTileIDLadder(int targetTileId)
    {
        for (int i = 0; i < laddersData.Count; i++)
        {
            if (laddersData[i].startTileId == targetTileId)
            {
                Debug.Log(targetTileId + " Is Ladder Start Tile Id" + laddersData[i].startTileId);
                return true;
            }
        }
        return false;
    }

    public bool IsTileIDSnake(int targetTileId)
    {
        for (int i = 0; i < snakesData.Count; i++)
        {
            if (snakesData[i].HeadSlotId == targetTileId)
            {
                Debug.Log(targetTileId + " Is Snake Head Tile Id" + snakesData[i].HeadSlotId);
                return true;
            }
        }
        return false;
    }

    public int GetLadderEndTileId(int startTileId)
    {
        for (int i = 0; i < laddersData.Count; i++)
        {
            if (laddersData[i].startTileId == startTileId)
            {
                return laddersData[i].endTileId;
            }
        }
        Debug.LogWarning("No Ladder Data Found In Given Slot Id :" + startTileId);
        return -1;
    }

    public int GetSnakeEndTileId(int startTileId)
    {
        SnakesData snake = snakesData.Where(s => s.HeadSlotId == startTileId).FirstOrDefault();
        if(snake != null)
        {
            return snake.TailSlotId;
        }
        else
        {
            Debug.LogWarning("No Snake Data Found In Given Slot Id :"+ startTileId);
            return -1;
        }
    }

    public PathCreator GetPathCreatorFromSnakeTailId(int standingTileId)
    {
        var snakeData = snakesData.Where(p =>(p.TailSlotId == standingTileId)).FirstOrDefault();

        if(snakeData != null)
        {
            Debug.Log("Target Tile Id" + standingTileId);
            return snakeData.SnakePathCreator;
        }

        snakeData = snakesData.Where(p => (p.TailSlotId == 7)).FirstOrDefault();
        return snakeData.SnakePathCreator; // Has Bug So Temporary Solution
    }

    public Vector3 GetStartPosition()
    {
        return tiles[0].transform.position;
    }

    [Serializable]
    public class LadderData
    {
        public int startTileId;
        public int endTileId;
    }

    [Serializable]
    public class SnakesData
    {
        public int HeadSlotId;
        public int TailSlotId;
        public PathCreator SnakePathCreator;
    }
}


