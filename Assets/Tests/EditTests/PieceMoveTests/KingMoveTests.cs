using System.Collections.Generic;
using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine;

namespace Castling.Tests.PieceMove
{
    [TestFixture]
    public class KingMoveTests
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
        public void King_ShouldMoveOneStepInAllDirections_EmptyBoard()
        {
            // Arrange
            int startX = 4, startY = 4;
            King king = new King { UID = "white_king", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = king;

            // Act
            List<Tile> moveableTiles = king.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 킹은 8방향으로 한 칸 이동할 수 있어야 함
            Assert.AreEqual(8, moveableTiles.Count, "킹의 이동 가능한 위치 수가 올바르지 않습니다.");
        }

        [Test]
        public void King_ShouldMoveLimitedOnBoardEdges()
        {
            // Arrange: 킹을 보드 가장자리(0, 0)에 배치
            int startX = 0, startY = 0;
            King king = new King { UID = "white_king", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = king;
            gameLogic.GameData.Board.Tiles[1, 1].Piece = null;
            gameLogic.GameData.Board.Tiles[1, 0].Piece = null;
            gameLogic.GameData.Board.Tiles[0, 1].Piece = null;

            // Act
            List<Tile> moveableTiles = king.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 보드 가장자리에서는 3개의 이동 가능한 위치만 있어야 함
            Assert.AreEqual(3, moveableTiles.Count, "킹의 이동 가능 범위가 가장자리에서 올바르지 않습니다.");
        }

        [Test]
        public void King_ShouldNotMoveToTileWithAllyPiece()
        {
            // Arrange: 킹 주변에 아군 기물을 배치
            int startX = 4, startY = 4;
            King king = new King { UID = "white_king", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = king;

            // 아군 기물 배치
            Piece allyPiece = new Piece { UID = "white_pawn_1", Color = TeamColor.White, Position = new Vector2Int(5, 4) };  // 오른쪽 위치
            gameLogic.GameData.Board.Tiles[5, 4].Piece = allyPiece;

            // Act
            List<Tile> moveableTiles = king.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 아군 기물이 있는 위치는 이동 불가
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(5, 4)), "아군 기물이 있는 위치로 이동할 수 있습니다.");
        }

        [Test]
        public void King_ShouldCaptureOpponentPiece()
        {
            // Arrange: 킹 주변에 상대 기물을 배치
            int startX = 4, startY = 4;
            King king = new King { UID = "white_king", Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = king;

            // 상대 기물 배치
            Piece enemyPiece = new Piece { UID = "black_pawn_1", Color = TeamColor.Black, Position = new Vector2Int(5, 4) };  // 오른쪽 위치
            gameLogic.GameData.Board.Tiles[5, 4].Piece = enemyPiece;

            // Act
            List<Tile> moveableTiles = king.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 상대 기물이 있는 위치는 이동 가능
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(5, 4)), "상대 기물이 있는 위치로 이동할 수 없습니다.");
        }
    }
}