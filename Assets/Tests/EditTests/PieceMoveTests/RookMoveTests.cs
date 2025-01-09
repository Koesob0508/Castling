using System.Collections.Generic;
using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine;

namespace Castling.Tests.PieceMove
{
    public class RookMoveTests
    {
        private DefaultGameLogic gameLogic;

        [SetUp]
        public void SetUp()
        {
            ulong blackClientID = 1;
            ulong whiteClientID = 2;
            gameLogic = new DefaultGameLogic(blackClientID, whiteClientID);
            gameLogic.Init();  // 보드 초기화
        }

        [Test]
        public void Rook_ShouldMoveHorizontallyAndVertically_InitBoard()
        {
            // Arrange
            int startX = 3, startY = 3;
            Rook rook = new Rook { UID = "white_rook", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = rook;

            // Act
            List<Tile> moveableTiles = rook.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 초기 보드에서는 최대 11개의 이동 가능한 위치가 있어야 함
            Assert.AreEqual(11, moveableTiles.Count, "룩의 이동 가능한 위치 수가 올바르지 않습니다.");
        }

        [Test]
        public void Rook_ShouldStopAtAllyPiece()
        {
            // Arrange
            int startX = 3, startY = 3;
            int allyX = 3, allyY = 6;  // 위쪽 방향에 아군 기물 배치
            Rook rook = new Rook { UID = "white_rook", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = rook;

            // 같은 팀 기물 배치
            Piece allyPiece = new Piece { UID = "white_pawn_1", Color = TeamColor.White, Position = new Vector2Int(allyX, allyY) };
            gameLogic.GameData.Board.Tiles[allyX, allyY].Piece = allyPiece;

            // Act
            List<Tile> moveableTiles = rook.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 아군 기물 위치는 이동할 수 없어야 함
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(allyX, allyY)), "아군 기물이 있는 위치로 이동할 수 있습니다.");
        }

        [Test]
        public void Rook_ShouldCaptureOpponentPiece()
        {
            // Arrange
            int startX = 3, startY = 3;
            int enemyX = 3, enemyY = 6;  // 위쪽 방향에 상대 기물 배치
            Rook rook = new Rook { UID = "white_rook", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = rook;

            // 상대 기물 배치
            Piece enemyPiece = new Piece { UID = "black_pawn_1", Color = TeamColor.Black, Position = new Vector2Int(enemyX, enemyY) };
            gameLogic.GameData.Board.Tiles[enemyX, enemyY].Piece = enemyPiece;

            // Act
            List<Tile> moveableTiles = rook.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 상대 기물이 있는 위치는 이동 가능하지만 그 이후는 불가능
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyX, enemyY)), "상대 기물이 있는 위치로 이동할 수 없습니다.");
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyX, enemyY + 1)), "상대 기물을 넘어서 이동할 수 있습니다.");
        }
    }
}