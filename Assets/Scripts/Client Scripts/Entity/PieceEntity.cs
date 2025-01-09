using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PieceEntity : MonoBehaviour
{
    private BoardEntity board;
    private string uid = string.Empty;
    private PieceType pieceID = 0;
    private eColor color;
    private Vector2Int position;

    public string UID { get => uid; private set => uid = value; }
    public PieceType PieceID { get => pieceID; private set => pieceID = value; }
    public eColor Color { get => color; private set => color = value; }
    public Vector2Int Position { get => position; private set => position = value; }

    private void Start()
    {
        board = Managers.Instance.Board;
    }

    public void Init(string UID, PieceType pieceID, eColor color, Vector2Int position)
    {
        this.UID = UID;
        this.color = color;
        this.position = position;
    }


    public virtual List<Vector2Int> GetMoveablePositions()
    {
        List<Vector2Int> moveablePositions = new List<Vector2Int>();
        
        if (color == eColor.Black)
        {
            moveablePositions.Add(new Vector2Int(Position.x - 1, Position.y));
            moveablePositions.Add(new Vector2Int(Position.x - 2, Position.y));
        }
        else
        {
            moveablePositions.Add(new Vector2Int(Position.x + 1, Position.y));
            moveablePositions.Add(new Vector2Int(Position.x + 2, Position.y));
        }

        return moveablePositions;
    }

    public void Move(Vector2Int destination)
    {
        Position = destination;
        transform.parent = board.GetTile(destination).transform;
        transform.localPosition = new Vector3(0, 1, 0);
    }
}

public enum eColor
{
    Black = 0,
    White = 1,
}