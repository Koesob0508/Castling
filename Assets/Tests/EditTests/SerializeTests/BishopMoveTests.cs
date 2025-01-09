using System.Collections.Generic;
using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine;

namespace Castling.Tests.PieceMove
{
    public class BishopMoveTests
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
        public void Bishop_ShouldMoveToAllAvailableDiagonalTiles_EmptyBoard()
        {
            // Arrange: 비숍을 특정 위치에 배치
            int startX = 3, startY = 3;
            string bishopUID = "test_bishop";
            Bishop bishop = new Bishop { UID = bishopUID, Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = bishop;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = bishop.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 빈 보드에서 대각선 방향으로 최대 이동 가능
            Assert.AreEqual(8, moveableTiles.Count, "대각선으로 이동 가능한 타일의 수가 올바르지 않습니다.");

            // 이동 가능한 좌표 리스트 확인
            foreach (Tile tile in moveableTiles)
            {
                Vector2Int pos = tile.Position;
                Assert.IsTrue(Mathf.Abs(pos.x - startX) == Mathf.Abs(pos.y - startY), $"대각선 위치가 아님: {pos}");
            }
        }

        [Test]
        public void Bishop_ShouldStopAtAllyPiece()
        {
            // Arrange: 비숍과 같은 팀 기물 배치
            int startX = 3, startY = 3;
            int allyX = 5, allyY = 5;
            string bishopUID = "test_bishop";
            Bishop bishop = new Bishop { UID = bishopUID, Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = bishop;

            // 같은 팀 기물 배치
            Piece allyPiece = new Piece { UID = "ally_piece", Color = TeamColor.White, Position = new Vector2Int(allyX, allyY) };
            gameLogic.GameData.Board.Tiles[allyX, allyY].Piece = allyPiece;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = bishop.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 아군 기물 위치까지 이동 가능하지만 해당 위치는 제외
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(allyX, allyY)), "같은 팀 기물이 있는 위치로 이동할 수 있습니다.");
        }

        [Test]
        public void Bishop_ShouldCaptureOpponentPiece()
        {
            // Arrange: 비숍과 상대 기물 배치
            int startX = 3, startY = 3;
            int enemyX = 5, enemyY = 5;
            string bishopUID = "test_bishop";
            Bishop bishop = new Bishop { UID = bishopUID, Color = TeamColor.White, Position = new Vector2Int(startX, startY) };
            gameLogic.GameData.Board.Tiles[startX, startY].Piece = bishop;

            // 상대 기물 배치
            Piece enemyPiece = new Piece { UID = "enemy_piece", Color = TeamColor.Black, Position = new Vector2Int(enemyX, enemyY) };
            gameLogic.GameData.Board.Tiles[enemyX, enemyY].Piece = enemyPiece;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = bishop.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 상대 기물 위치는 이동 가능하지만 그 이후는 이동 불가
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyX, enemyY)), "상대 기물이 있는 위치로 이동할 수 없습니다.");
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyX + 1, enemyY + 1)), "상대 기물을 넘어서 이동할 수 없습니다.");
        }
    }
}