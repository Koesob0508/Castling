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
    private PieceType pieceID = 0;
    private TeamColor color;
    private Vector2Int position;

    public string UID { get => uid; private set => uid = value; }
    public PieceType PieceID { get => pieceID; private set => pieceID = value; }
    public TeamColor Color { get => color; private set => color = value; }
    public Vector2Int Position { get => position; private set => position = value; }

    private void Start()
    {
        board = Managers.Instance.Board;
    }

    public void Init(string UID, PieceType pieceID, TeamColor color, Vector2Int position)
    {
        isDie = false;
        this.UID = UID;
        this.color = color;
        this.position = position;
    }


    public virtual List<Vector2Int> GetMoveablePositions()
    {
        List<Vector2Int> moveablePositions = new List<Vector2Int>();
        
        if (color == TeamColor.Black)
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

        StartCoroutine(MoveToPosition(destination, 0.5f));
    }

    private IEnumerator MoveToPosition(Vector2Int destination, float duration)
    {
        isMoving = true;
        Position = destination;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(destination.x, 1f, destination.y);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // 프레임 시간을 누적
            yield return null; // 다음 프레임까지 대기
        }

        isMoving = false;
        transform.parent = board.GetTile(destination).transform;
        transform.localPosition = new Vector3(0, 1, 0);
    }

    public void Die()
    {
        isDie = true;
        position = new(10, 10);
        gameObject.SetActive(false);
    }
}