using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardEntity : MonoBehaviour
{
    public TileEntity TileBlack;
    public TileEntity TileWhite;

    public int xSize = 8;
    public int ySize = 8;

    [SerializeField]

    private TileEntity[,] tiles;
    [SerializeField]
    private List<PieceEntity> pieces;

    public void Start()
    {
        tiles = new TileEntity[xSize, ySize];
        CreateBoard();
        CreatePieces();
    }

    public void Init()
    {

    }


    private void CreateBoard()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                TileEntity tile;

                if ((x + y) % 2 == 0)
                    tile = Instantiate(TileBlack, transform);
                else
                    tile = Instantiate(TileWhite, transform);

                tile.Init(x, y);
                tile.name = $"Tile ({x},{y})";
                tile.transform.position = new Vector3(x, 0, y);

                tiles[x, y] = tile;
            }
        }
    }

    private void CreatePieces()
    {
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 0);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 1);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 2);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 3);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 4);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 5);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 6);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.White, 1, 7);

        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 0);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 1);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 2);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 3);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 4);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 5);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 6);
        CreatePiece("Pawn", Guid.NewGuid().ToString(), 0, eColor.Black, 6, 7);
    }

    private void CreatePiece(string prefabName, string UID, int pieceID, eColor color, int x, int y)
    {
        string pieceName = $"Piece_{prefabName}_{color}";
        PieceEntity piecePrefab = Resources.Load<PieceEntity>($"Prefabs/{pieceName}");

        if (piecePrefab == null)
        {
            Debug.LogError($"{prefabName} does not exist.");
            return;
        }

        PieceEntity piece = Instantiate(piecePrefab);
        piece.UID = UID;
        piece.Color = color;
        piece.transform.parent = tiles[x, y].transform;
        piece.transform.localPosition = new Vector3(0, 1, 0);
    }

    private void MovePiece(string UID, int toX, int toY)
    {

    }

    public void ShowMoveableTiles(List<Vector2Int> positions)
    {
        foreach (Vector2Int position in positions)
        {
            tiles[position.x, position.y].ShowMoveableEffect();
        }
    }

    public void HideMoveableTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].HideMoveableEffect();
            }
        }
    }

}
