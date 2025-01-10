using Castling.Shared;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PieceEntity : MonoBehaviour
{
    private bool isDie = true;
    private bool isMoving = false;
    private BoardEntity board;
    private string uid = string.Empty;
    private PieceType pieceType = 0;
    private TeamColor color;
    private Vector2Int position;
    private Piece pieceModel;

    public string UID { get => uid; private set => uid = value; }
    public PieceType PieceID { get => pieceType; private set => pieceType = value; }
    public TeamColor Color { get => color; private set => color = value; }
    public Vector2Int Position { get => position; private set => position = value; }

    private void Start()
    {
        board = Managers.Instance.Board;
    }

    public void Init(string UID, PieceType pieceType, TeamColor color, TileEntity tileEntity)
    {
        isDie = false;
        this.UID = UID;
        this.pieceType = pieceType;
        this.color = color;
        this.position = tileEntity.Position;
        SetPositionAtTile(tileEntity);

        switch (pieceType)
        {
            case PieceType.None:
                break;
            case PieceType.Pawn:
                pieceModel = new Pawn() { Color = color };
                break;
            case PieceType.King:
                pieceModel = new King() { Color = color };
                break;
            case PieceType.Queen:
                pieceModel = new Queen() { Color = color };
                break;
            case PieceType.Bishop:
                pieceModel = new Bishop() { Color = color };
                break;
            case PieceType.Knight:
                pieceModel = new Knight() { Color = color };
                break;
            case PieceType.Rook:
                pieceModel = new Rook() { Color = color };
                break;
        }
    }

    public void SetPositionAtTile(TileEntity tileEntity)
    {
        transform.parent = tileEntity.transform;
        transform.localPosition = new Vector3(0, 0.5f, 0);
    }

    public virtual List<Tile> GetMoveablePositions()
    {
        return pieceModel.GetMoveableTiles(Managers.Instance.GameData, position.x, position.y);
    }

    public void Move(Vector2Int destination)
    {
        Debug.Log("Move : " + name + ", " + Position + ", " + destination);
        StartCoroutine(MoveToPosition(destination, 0.5f));
    }

    private IEnumerator MoveToPosition(Vector2Int destination, float duration)
    {
        isMoving = true;
        Position = destination;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(destination.x, 0.5f, destination.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // 프레임 시간을 누적
            yield return null; // 다음 프레임까지 대기
        }

        isMoving = false;
        SetPositionAtTile(board.GetTile(destination));
    }

    public void Die()
    {
        Debug.Log("Die :" + name);
        isDie = true;
        position = new(10, 10);
        gameObject.SetActive(false);
    }
}