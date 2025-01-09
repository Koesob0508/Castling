using System.Collections.Generic;
using UnityEngine;

namespace Castling.Shared
{
    public class Bishop : Piece
    {
        public override List<Tile> GetMoveableTiles(GameData gameData, int currentX, int currentY)
        {
            List<Tile> moveableTiles = new List<Tile>();

            // 대각선 방향 벡터 (좌상, 우상, 좌하, 우하)
            Vector2Int[] directions = {
            new Vector2Int(-1, -1), // 왼쪽 위 대각선
            new Vector2Int(1, -1),  // 오른쪽 위 대각선
            new Vector2Int(-1, 1),  // 왼쪽 아래 대각선
            new Vector2Int(1, 1)    // 오른쪽 아래 대각선
        };

            foreach (var direction in directions)
            {
                int x = currentX + direction.x;
                int y = currentY + direction.y;

                // 보드 경계를 벗어나지 않는 한 계속 탐색
                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    Tile tile = gameData.Board.Tiles[x, y];

                    // 해당 위치가 비어 있으면 이동 가능
                    if (tile.Piece == null)
                    {
                        moveableTiles.Add(tile);
                    }
                    // 상대 기물이 있으면 해당 위치까지 이동 가능하고, 그 이후는 불가능
                    else if (tile.Piece.Color != Color)
                    {
                        moveableTiles.Add(tile);
                        break;
                    }
                    // 같은 팀 기물이 있으면 해당 방향으로 더 이상 이동 불가
                    else
                    {
                        break;
                    }

                    // 다음 대각선 위치로 진행
                    x += direction.x;
                    y += direction.y;
                }
            }

            return moveableTiles;
        }
    }
}