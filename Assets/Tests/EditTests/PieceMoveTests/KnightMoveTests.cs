using System.Collections.Generic;
using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine;

namespace Castling.Tests.PieceMove
{
    [TestFixture]
    public class KnightMoveTests
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
        public void WhiteKnight_ShouldMoveToAllLShapedPositions_InitBoard()
        {
            // Arrange
            int startX = 4, startY = 4;
            Knight knight = new Knight { UID = "white_knight", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = knight;

            // Act
            List<Tile> moveableTiles = knight.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 초기 보드에서는 6개의 이동 가능한 위치가 있어야 함
            Assert.AreEqual(6, moveableTiles.Count, "나이트의 이동 가능한 위치 수가 올바르지 않습니다.");
        }

        [Test]
        public void BlackKnight_ShouldMoveToAllLShapedPositions_InitBoard()
        {
            // Arrange
            int startX = 4, startY = 4;
            Knight knight = new Knight { UID = "white_knight", Color = TeamColor.Black, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = knight;

            // Act
            List<Tile> moveableTiles = knight.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 초기 보드에서는 8개의 이동 가능한 위치가 있어야 함
            Assert.AreEqual(8, moveableTiles.Count, "나이트의 이동 가능한 위치 수가 올바르지 않습니다.");
        }

        [Test]
        public void Knight_ShouldNotMoveToTileWithAllyPiece()
        {
            // Arrange
            int startX = 4, startY = 4;
            int allyX = 2, allyY = 3;  // 이동할 수 있는 L자 위치 중 하나
            Knight knight = new Knight { UID = "white_knight", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = knight;

            // 같은 팀 기물 배치
            Piece allyPiece = new Piece { UID = "white_pawn_1", Color = TeamColor.White, Position = new Vector2Int(allyX, allyY) };
            gameLogic.GameData.Board.Tiles[allyX, allyY].Piece = allyPiece;

            // Act
            List<Tile> moveableTiles = knight.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 아군 기물이 있는 위치는 이동 불가
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(allyX, allyY)), "아군 기물이 있는 위치로 이동할 수 있습니다.");
        }

        [Test]
        public void Knight_ShouldCaptureOpponentPiece()
        {
            // Arrange
            int startX = 4, startY = 4;
            int enemyX = 2, enemyY = 3;  // 이동할 수 있는 L자 위치 중 하나
            Knight knight = new Knight { UID = "white_knight", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = knight;

            // 상대 기물 배치
            Piece enemyPiece = new Piece { UID = "black_pawn_1", Color = TeamColor.Black, Position = new Vector2Int(enemyX, enemyY) };
            gameLogic.GameData.Board.Tiles[enemyX, enemyY].Piece = enemyPiece;

            // Act
            List<Tile> moveableTiles = knight.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 상대 기물이 있는 위치는 이동 가능
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyX, enemyY)), "상대 기물이 있는 위치로 이동할 수 없습니다.");
        }
    }
}