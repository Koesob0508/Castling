using System.Collections.Generic;
using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine;

namespace Castling.Tests.PieceMove
{

    [TestFixture]
    public class WhitePawnMoveTests
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
        public void WhitePawn_ShouldMoveOneStepForward_WhenTileIsEmpty()
        {
            // Arrange: 흰 폰을 기본 위치에 배치
            int startX = 3, startY = 6; // 기본 흰 폰 위치 (3, 6)
            string pawnUID = "white_pawn_3";
            Piece whitePawn = gameLogic.GameData.Board.Tiles[startX, startY].Piece;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = whitePawn.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 1칸 전진이 포함되어 있어야 함
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(startX, startY - 1)), "1칸 전진 이동이 포함되지 않았습니다.");
        }

        [Test]
        public void WhitePawn_ShouldMoveTwoStepsForward_OnlyOnFirstMove()
        {
            // Arrange: 흰 폰을 기본 위치에 배치
            int startX = 3, startY = 6; // 기본 흰 폰 위치 (3, 6)
            string pawnUID = "white_pawn_3";
            Piece whitePawn = gameLogic.GameData.Board.Tiles[startX, startY].Piece;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = whitePawn.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 2칸 전진이 포함되어 있어야 함 (처음 위치에서만 가능)
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(startX, startY - 2)), "2칸 전진 이동이 포함되지 않았습니다.");
        }

        [Test]
        public void WhitePawn_ShouldNotMoveBackward()
        {
            // Arrange: 흰 폰을 기본 위치에 배치
            int startX = 3, startY = 6; // 기본 흰 폰 위치 (3, 6)
            string pawnUID = "white_pawn_3";
            Piece whitePawn = gameLogic.GameData.Board.Tiles[startX, startY].Piece;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = whitePawn.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 후진(뒤로 이동)이 없어야 함
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(startX, startY + 1)), "폰이 뒤로 이동할 수 있습니다.");
        }

        [Test]
        public void WhitePawn_ShouldCaptureOpponentDiagonally()
        {
            // Arrange: 대각선에 상대 기물을 배치
            int startX = 3, startY = 6; // 기본 흰 폰 위치
            int enemyXLeft = 2, enemyY = 5;  // 왼쪽 대각선
            int enemyXRight = 4; // 오른쪽 대각선

            string pawnUID = "white_pawn_3";
            Piece whitePawn = gameLogic.GameData.Board.Tiles[startX, startY].Piece;

            // 상대 기물 배치
            Piece enemyLeft = new Piece { UID = "black_piece_1", Color = TeamColor.Black, Position = new Vector2Int(enemyXLeft, enemyY) };
            Piece enemyRight = new Piece { UID = "black_piece_2", Color = TeamColor.Black, Position = new Vector2Int(enemyXRight, enemyY) };

            gameLogic.GameData.Board.Tiles[enemyXLeft, enemyY].Piece = enemyLeft;
            gameLogic.GameData.Board.Tiles[enemyXRight, enemyY].Piece = enemyRight;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = whitePawn.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 대각선 방향 공격이 가능해야 함
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyXLeft, enemyY)), "왼쪽 대각선 공격이 불가능합니다.");
            Assert.IsTrue(moveableTiles.Exists(tile => tile.Position == new Vector2Int(enemyXRight, enemyY)), "오른쪽 대각선 공격이 불가능합니다.");
        }

        [Test]
        public void WhitePawn_ShouldNotCaptureAllyPieceDiagonally()
        {
            // Arrange: 대각선에 같은 팀 기물 배치
            int startX = 3, startY = 6; // 기본 흰 폰 위치
            int allyXLeft = 2, allyY = 5;  // 왼쪽 대각선
            int allyXRight = 4; // 오른쪽 대각선

            string pawnUID = "white_pawn_3";
            Piece whitePawn = gameLogic.GameData.Board.Tiles[startX, startY].Piece;

            // 같은 팀 기물 배치
            Piece allyLeft = new Piece { UID = "white_piece_1", Color = TeamColor.White, Position = new Vector2Int(allyXLeft, allyY) };
            Piece allyRight = new Piece { UID = "white_piece_2", Color = TeamColor.White, Position = new Vector2Int(allyXRight, allyY) };

            gameLogic.GameData.Board.Tiles[allyXLeft, allyY].Piece = allyLeft;
            gameLogic.GameData.Board.Tiles[allyXRight, allyY].Piece = allyRight;

            // Act: 가능한 이동 타일 가져오기
            List<Tile> moveableTiles = whitePawn.GetMoveableTiles(gameLogic.GameData, startX, startY);

            // Assert: 같은 팀 기물이 있는 대각선 방향으로는 이동할 수 없어야 함
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(allyXLeft, allyY)), "왼쪽 대각선에 있는 아군 기물을 잡을 수 있습니다.");
            Assert.IsFalse(moveableTiles.Exists(tile => tile.Position == new Vector2Int(allyXRight, allyY)), "오른쪽 대각선에 있는 아군 기물을 잡을 수 있습니다.");
        }


    }
}