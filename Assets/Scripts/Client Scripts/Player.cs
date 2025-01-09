using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private ulong clientID;
    private eColor color;

    [SerializeField]
    private BoardEntity board;
    [SerializeField]
    private PieceEntity? capturedPiece;
    [SerializeField]
    private GameObject cursor;

    private void Start()
    {
        board = Managers.Instance.Board;
    }

    public void Init(ulong clientID, eColor color)
    {
        this.clientID = clientID;
        this.color = color;
    }

    private void Update()
    {
        // 마우스 클릭 입력
        if (Input.GetMouseButtonDown(0))
        {
            // 카메라에서 마우스 포인터 방향으로 레이 생성
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 충돌 정보를 저장할 RaycastHit 변수
            RaycastHit hit;

            // Raycast 실행
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌한 오브젝트 정보 출력
                Debug.Log($"Hit Object: {hit.collider.gameObject.name}");

                if (hit.collider.TryGetComponent(out PieceEntity piece))
                {
                    if (piece.Color == this.color)
                        CapturePiece(piece);
                    else
                        UnCapturePiece();
                }
                else if (hit.collider.TryGetComponent(out TileEntity tile))
                {

                    if (tile.IsMoveable)
                    {
                        var targetPosition = capturedPiece.GetMoveablePositions()
                            .Find(pos => pos == tile.Position);

                        if (targetPosition != null)
                        {
                            capturedPiece.Move(targetPosition);
                            UnCapturePiece();
                        }
                    }
                }
                else
                {
                    UnCapturePiece();
                }



            }
            else
            {
                Debug.Log("No object hit.");
            }
        }

        if (Input.GetMouseButtonDown (1))
        {
            UnCapturePiece ();
        }
    }

    private void CapturePiece(PieceEntity piece)
    {
        if (capturedPiece) UnCapturePiece();
        capturedPiece = piece;

        Vector3 cursorPosition = piece.transform.position;
        cursorPosition.y += 1.5f;
        cursor.transform.position = cursorPosition;
        cursor.SetActive(true);

        List<Vector2Int> moveablePositions = capturedPiece.GetMoveablePositions();
        board.ShowMoveableTiles(moveablePositions);
    }

    private void UnCapturePiece()
    {
        if (capturedPiece == null) return;

        cursor.SetActive(false);
        capturedPiece = null;
        board.HideMoveableTiles();
    }
}