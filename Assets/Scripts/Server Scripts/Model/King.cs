using System.Collections.Generic;
using UnityEngine;

namespace Castling.Shared
{
    public class King : Piece
    {
        // 킹이 한 번이라도 움직였는지 여부
        public bool HasMoved { get; private set; } = false;

        public override List<Tile> GetMoveableTiles(GameData gameData, int currentX, int currentY)
        {
            List<Tile> moveableTiles = new List<Tile>();

            // 킹의 기본 8방향 이동
            Vector2Int[] directions = {
            new Vector2Int(0, 1),   // 위
            new Vector2Int(0, -1),  // 아래
            new Vector2Int(1, 0),   // 오른쪽
            new Vector2Int(-1, 0),  // 왼쪽
            new Vector2Int(-1, -1), // 왼쪽 위 대각선
            new Vector2Int(1, -1),  // 오른쪽 위 대각선
            new Vector2Int(-1, 1),  // 왼쪽 아래 대각선
            new Vector2Int(1, 1)    // 오른쪽 아래 대각선
        };

            foreach (var direction in directions)
            {
                int newX = currentX + direction.x;
                int newY = currentY + direction.y;

                if (IsWithinBounds(newX, newY))
                {
                    Tile tile = gameData.Board.Tiles[newX, newY];
                    if (tile.Piece == null || tile.Piece.Color != this.Color)
                    {
                        moveableTiles.Add(tile);
                    }
                }
            }

            // 캐슬링 추가
            //if (!HasMoved)
            //{
            //    AddCastlingMoves(gameData, moveableTiles, currentX, currentY);
            //}

            return moveableTiles;
        }

        #region Castling 관련 로직

        //private void AddCastlingMoves(GameData gameData, List<Tile> moveableTiles, int currentX, int currentY)
        //{
        //    // 왼쪽(퀸사이드) 캐슬링
        //    if (CanCastleLeft(gameData, currentX, currentY))
        //    {
        //        moveableTiles.Add(gameData.Board.Tiles[currentX - 2, currentY]);
        //    }

        //    // 오른쪽(킹사이드) 캐슬링
        //    if (CanCastleRight(gameData, currentX, currentY))
        //    {
        //        moveableTiles.Add(gameData.Board.Tiles[currentX + 2, currentY]);
        //    }
        //}

        //private bool CanCastleLeft(GameData gameData, int currentX, int currentY)
        //{
        //    // 퀸사이드 룩의 위치
        //    Tile rookTile = gameData.Board.Tiles[0, currentY];
        //    Piece rook = rookTile?.Piece;

        //    if (rook is Rook && rook.Color == this.Color && !((Rook)rook).HasMoved)
        //    {
        //        // 킹과 룩 사이에 기물이 없어야 함
        //        for (int x = currentX - 1; x > 0; x--)
        //        {
        //            if (gameData.Board.Tiles[x, currentY].Piece != null)
        //            {
        //                return false;
        //            }
        //        }
        //        // 체크 상태가 아니어야 함
        //        if (!IsUnderAttack(gameData, currentX - 1, currentY) && !IsUnderAttack(gameData, currentX - 2, currentY))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private bool CanCastleRight(GameData gameData, int currentX, int currentY)
        //{
        //    // 킹사이드 룩의 위치
        //    Tile rookTile = gameData.Board.Tiles[7, currentY];
        //    Piece rook = rookTile?.Piece;

        //    if (rook is Rook && rook.Color == this.Color && !((Rook)rook).HasMoved)
        //    {
        //        // 킹과 룩 사이에 기물이 없어야 함
        //        for (int x = currentX + 1; x < 7; x++)
        //        {
        //            if (gameData.Board.Tiles[x, currentY].Piece != null)
        //            {
        //                return false;
        //            }
        //        }
        //        // 체크 상태가 아니어야 함
        //        if (!IsUnderAttack(gameData, currentX + 1, currentY) && !IsUnderAttack(gameData, currentX + 2, currentY))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private bool IsUnderAttack(GameData gameData, int x, int y)
        //{
        //    // 보드 전체를 스캔하여 상대 기물이 해당 위치를 공격할 수 있는지 확인
        //    foreach (var piece in gameData.Board.Pieces)
        //    {
        //        if (piece.Color != this.Color)
        //        {
        //            List<Tile> opponentMoves = piece.GetMoveableTiles(gameData, piece.Position.x, piece.Position.y);
        //            if (opponentMoves.Exists(tile => tile.Position == new Vector2Int(x, y)))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //public void SetHasMoved(bool hasMoved)
        //{
        //    HasMoved = hasMoved;
        //}
        #endregion

        private bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }
}