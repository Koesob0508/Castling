using System.Collections.Generic;
using UnityEngine;

namespace Castling.Shared
{
    public class Knight : Piece
    {
        public override List<Tile> GetMoveableTiles(GameData gameData, int currentX, int currentY)
        {
            List<Tile> moveableTiles = new List<Tile>();

            // 나이트의 이동 가능한 모든 상대적 위치
            Vector2Int[] moves = new Vector2Int[]
            {
            new Vector2Int(-2, -1), new Vector2Int(-2, 1),
            new Vector2Int(-1, -2), new Vector2Int(-1, 2),
            new Vector2Int(1, -2), new Vector2Int(1, 2),
            new Vector2Int(2, -1), new Vector2Int(2, 1)
            };

            foreach (var move in moves)
            {
                int newX = currentX + move.x;
                int newY = currentY + move.y;

                // 보드 경계 검사
                if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8)
                    continue;

                Tile targetTile = gameData.Board.Tiles[newX, newY];

                // 타일이 비어 있거나 상대방 기물이 있는 경우 이동 가능
                if (targetTile.Piece == null || targetTile.Piece.Color != this.Color)
                {
                    moveableTiles.Add(targetTile);
                }
            }

            return moveableTiles;
        }
    }
}