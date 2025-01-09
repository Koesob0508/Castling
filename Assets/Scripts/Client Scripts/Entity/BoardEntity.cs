using Castling.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardEntity : MonoBehaviour
{
    public TileEntity TileBlack;
    public TileEntity TileWhite;

    public int xSize = 8;
    public int ySize = 8;

    [SerializeField]

    private TileEntity[,] tiles;
    [SerializeField]
    private Dictionary<string, PieceEntity> pieces = new Dictionary<string, PieceEntity>();

    public TileEntity GetTile(Vector2Int position) => tiles[position.x, position.y];
    public PieceEntity GetPiece(string uid) => pieces[uid];

    public GameData testData;

    private void Start()
    {
        
    }

    public void Init(GameData gameData)
    {
        xSize = gameData.Board.xSize;
        ySize = gameData.Board.ySize;
        tiles = new TileEntity[xSize, ySize];

        CreateBoard();
        CreatePieces(gameData);
    }

    public void BoardTestData(string actorID, Vector2Int destination)
    {
        testData = new GameData();

        testData.BlackClientID = 0;
        testData.WhiteClientID = 0;

        testData.Board = new Board();
        testData.Board.xSize = xSize;
        testData.Board.ySize = ySize;

        testData.Board.Tiles = new Tile[xSize, ySize];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                Tile tile = new Tile();
                tile.Position = tiles[i, j].Position;

                testData.Board.Tiles[i, j] = tile;
            }
        }

        testData.Board.Pieces = new List<Piece>();
        foreach (PieceEntity pieceEntity in pieces.Values)
        {
            Piece piece = new Piece();

            if (pieceEntity.UID == actorID)
            {
                piece.UID = pieceEntity.UID;
                piece.Position = destination;
            }
            else
            {
                piece.UID = pieceEntity.UID;
                piece.Position = pieceEntity.Position;

                
            }

            testData.Board.Pieces.Add(piece);
        }

        Piece pie = null;
        foreach (Piece p in testData.Board.Pieces)
        {
            if (p.UID != actorID)
            {
                if (p.Position == destination)
                    pie = p;
            }
        }

        if (pie != null) testData.Board.Pieces.Remove(pie);

        MovePieceResult(testData.Board.Pieces.ToDictionary(x => x.UID, x => x));
    }


    private void CreateBoard()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                TileEntity tilePrefab;

                if ((x + y) % 2 == 0)
                    tilePrefab = Resources.Load<TileEntity>($"Prefabs/Tile_Black");
                else
                    tilePrefab = Resources.Load<TileEntity>($"Prefabs/Tile_White");

                TileEntity tile = Instantiate(tilePrefab, transform);
                tile.Init(x, y);
                tile.name = $"Tile ({x},{y})";
                tile.transform.position = new Vector3(x, 0, y);

                tiles[x, y] = tile;
            }
        }
    }

    private void CreatePieces(GameData gameData)
    {
        foreach (var piece in gameData.Board.Pieces)
        {
            string name = piece.Type.ToString();
            string UID = piece.UID;
            PieceType pieceType = piece.Type;
            Vector2Int position = piece.Position;
            TeamColor color = piece.Color;

            CreatePiece(name, UID, pieceType, color, position);
        }
    }

    private void CreatePiece(string prefabName, string UID, PieceType pieceID, TeamColor color, Vector2Int position)
    {
        string pieceName = $"{color} {prefabName}";
        PieceEntity piecePrefab = Resources.Load<PieceEntity>($"Prefabs/{pieceName}");

        if (piecePrefab == null)
        {
            Debug.LogError($"{pieceName} does not exist.");
            return;
        }

        PieceEntity piece = Instantiate(piecePrefab);
        piece.Init(UID, pieceID, color, position);
        piece.transform.parent = tiles[position.x, position.y].transform;
        piece.transform.localPosition = new Vector3(0, 1, 0);
        pieces.Add(UID, piece);
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

    public void MovePieceResult(Dictionary<string, Piece> piecesResult)
    {
        Debug.Log("MovePieceResult");

        foreach(var pieceEntityPair in pieces)
        {
            if (piecesResult.ContainsKey(pieceEntityPair.Key))
            {
                Piece pieceModel = piecesResult[pieceEntityPair.Key];
                PieceEntity pieceEntity = pieceEntityPair.Value;

                if (pieceEntity.Position != pieceModel.Position)
                    pieceEntity.Move(pieceModel.Position);
            }
            else
            {
                PieceEntity pieceEntity = pieceEntityPair.Value;
                pieceEntity.Die();
            }
        }
    }
}
