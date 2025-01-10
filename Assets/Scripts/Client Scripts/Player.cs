using Castling.Shared;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private ulong clientID;
    private TeamColor color;

    [SerializeField]
    private BoardEntity board;
    [SerializeField]
    private PieceEntity? capturedPiece;
    [SerializeField]
    private GameObject cursor;

    public void Init(ulong clientID, TeamColor color)
    {
        board = Managers.Instance.Board;
        this.clientID = clientID;
        this.color = color;
    }

    private void Update()
    {
        // gamedata 에는 직전 행동의 정보가 담김.
        // if (Managers.Instance.GameData.CurrentClientID == clientID) return;

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
                        var destinationTile = capturedPiece.GetMoveablePositions()
                            .Find(moveableTile => moveableTile.Position == tile.Position);

                        if (destinationTile != null)
                        {
                            // 서버에게 전송하기
                            Managers.Instance.Client
                                .SendTryMovePiece(capturedPiece.UID, destinationTile.Position);

                            // 테스트 중
                            //board.BoardTestData(capturedPiece.UID, targetPosition);
                        }
                    }
                    UnCapturePiece();
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

        cursor.transform.parent = capturedPiece.transform;
        cursor.transform.localPosition = new Vector3(0, 1.2f, 0);
        cursor.SetActive(true);

        List<Tile> moveablePositions = capturedPiece.GetMoveablePositions();
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