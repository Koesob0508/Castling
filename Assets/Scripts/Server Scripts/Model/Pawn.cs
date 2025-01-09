using Castling.Shared;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Tile> GetMoveableTiles(int currentX, int currentY)
    {
        List<Tile> moveableTiles = new List<Tile>();

        // 현재 폰의 방향: 화이트는 위로(감소), 블랙은 아래로(증가)
        int direction = Color == TeamColor.White ? -1 : 1;

        // 전진 1칸 이동
        Vector2Int forwardOne = new Vector2Int(currentX, currentY + direction);
        if (IsTileEmpty(forwardOne))
        {
            moveableTiles.Add(new Tile { Position = forwardOne });
        }

        // 첫 이동 시 전진 2칸 이동
        int startingY = Color == TeamColor.White ? 6 : 1;
        if (currentY == startingY && IsTileEmpty(forwardOne))
        {
            Vector2Int forwardTwo = new Vector2Int(currentX, currentY + 2 * direction);
            if (IsTileEmpty(forwardTwo))
            {
                moveableTiles.Add(new Tile { Position = forwardTwo });
            }
        }

        // 대각선 공격 (왼쪽, 오른쪽)
        Vector2Int[] diagonalAttacks = {
            new Vector2Int(currentX - 1, currentY + direction),  // 왼쪽 대각선
            new Vector2Int(currentX + 1, currentY + direction)   // 오른쪽 대각선
        };

        foreach (var attackPos in diagonalAttacks)
        {
            if (IsOpponentPiece(attackPos))
            {
                moveableTiles.Add(new Tile { Position = attackPos });
            }
        }

        return moveableTiles;
    }

    // 해당 타일이 비어 있는지 확인
    private bool IsTileEmpty(Vector2Int pos)
    {
        Tile tile = Logic.GameData.Board.Tiles[pos.x, pos.y];
        return tile != null && tile.Piece == null;
    }

    // 해당 위치에 상대방 기물이 있는지 확인
    private bool IsOpponentPiece(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= Logic.GameData.Board.xSize || pos.y < 0 || pos.y >= Logic.GameData.Board.ySize)
            return false;

        Tile tile = Logic.GameData.Board.Tiles[pos.x, pos.y];
        return tile?.Piece != null && tile.Piece.Color != Color;
    }
}