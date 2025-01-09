using System.Collections.Generic;
using UnityEngine;

namespace Castling.Shared
{
    public class Queen : Piece
    {
        public override List<Tile> GetMoveableTiles(GameData gameData, int currentX, int currentY)
        {
            List<Tile> moveableTiles = new List<Tile>();

            // 퀸의 이동 가능한 모든 방향 (상, 하, 좌, 우 + 대각선)
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
                int x = currentX + direction.x;
                int y = currentY + direction.y;

                // 해당 방향으로 최대 거리까지 탐색
                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    Tile tile = gameData.Board.Tiles[x, y];

                    // 타일이 비어 있으면 이동 가능
                    if (tile.Piece == null)
                    {
                        moveableTiles.Add(tile);
                    }
                    // 상대 기물이 있는 경우 해당 위치까지 이동 가능하고 그 이후는 불가능
                    else if (tile.Piece.Color != this.Color)
                    {
                        moveableTiles.Add(tile);
                        break;
                    }
                    // 아군 기물이 있는 경우 해당 방향 탐색 종료
                    else
                    {
                        break;
                    }

                    // 다음 위치로 진행
                    x += direction.x;
                    y += direction.y;
                }
            }

            return moveableTiles;
        }
    }
}