using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int tileId;

    public int GetTileId()
    {
        return tileId;
    }

    public void SetTileId(int tileId)
    {
        this.tileId = tileId;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}
