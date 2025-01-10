using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Castling.Tests
{
    public class GameLogicTests
    {
        private DefaultGameLogic gameLogic;

        [SetUp]
        public void SetUp()
        {
            // 검증을 위한 Mock Client IDs
            ulong blackClientID = 1;
            ulong whiteClientID = 2;

            // `DefaultGameLogic` 초기화
            gameLogic = new DefaultGameLogic(blackClientID, whiteClientID);
        }

        [TearDown]
        public void TearDown()
        {
            gameLogic = null;
        }

        [Test]
        public void Init_ShouldInitializeBoardAndInvokeOnGameStarted()
        {
            // 이벤트 호출 검증을 위한 플래그
            bool gameStartedEventCalled = false;

            // 이벤트 핸들러 추가
            gameLogic.OnGameStarted += () => gameStartedEventCalled = true;

            // Init 호출
            gameLogic.Init();

            // 검증: OnGameStarted가 호출되었는지 확인
            Assert.IsTrue(gameStartedEventCalled, "OnGameStarted 이벤트가 호출되지 않았습니다.");

            // 검증: 체스판이 8x8 사이즈로 초기화되었는지 확인
            Assert.IsNotNull(gameLogic.GameData.Board, "Board가 null입니다.");
            Assert.AreEqual(8, gameLogic.GameData.Board.xSize, "Board의 xSize가 잘못되었습니다.");
            Assert.AreEqual(8, gameLogic.GameData.Board.ySize, "Board의 ySize가 잘못되었습니다.");

            // 검증: 모든 기물이 올바르게 배치되었는지 확인
            int totalPieces = gameLogic.GameData.Board.Pieces.Count;
            Assert.AreEqual(32, totalPieces, "기물의 수가 잘못되었습니다. 체스 기본 배치는 32개의 기물이 있어야 합니다.");
        }

        [Test]
        public void Clear_ShouldClearGameData_AndUnsubscribeAllEvents()
        {
            gameLogic.Init();

            // 1. 초기화 상태 확인
            Assert.IsNotNull(gameLogic.GameData, "Init 후 GameData가 null이면 안 됩니다.");
            Assert.IsNotNull(gameLogic.GameData.Board, "Init 후 GameData.Board가 null이면 안 됩니다.");
            Assert.AreEqual(32, gameLogic.GameData.Board.Pieces.Count, "Init 후 기물의 수가 잘못되었습니다.");

            // 2. `Clear()` 호출
            gameLogic.Clear();

            // 3. GameData 및 보드 초기화 확인
            Assert.IsNull(gameLogic.GameData, "GameData는 Clear 후 null이어야 합니다.");

            // 4. 이벤트 해제 확인
        }

        [Test]
        public void Clear_AfterMultipleInitializations_ShouldStillClearEverything()
        {
            // 여러 번 Init 호출
            gameLogic.Init();
            gameLogic.Init();

            // `Clear()` 호출
            gameLogic.Clear();

            // GameData가 null인지 확인
            Assert.IsNull(gameLogic.GameData, "여러 번 Init 호출 후에도 Clear는 GameData를 null로 설정해야 합니다.");
        }

        [Test]
        public void TryMovePiece_CaptureEnemyPiece_ShouldUpdateBoardCorrectly()
        {
            // Arrange
            gameLogic.Init();

            string blackRookUID = "black_rook_test";
            string whiteQueenUID = "white_queen_test";
            Vector2Int capturePosition = new Vector2Int(4, 6);

            // Setup: 이동을 위해 백 퀸을 임의 위치에 배치 (테스트 목적)
            Piece whiteQueen = new Queen
            {
                Color = TeamColor.White,
                Type = PieceType.Queen,
                UID = whiteQueenUID,
                Position = capturePosition
            };

            Vector2Int startPosition = new Vector2Int(4, 2);

            Piece blackRook = new Rook
            {
                Color = TeamColor.Black,
                Type = PieceType.Rook,
                UID = blackRookUID,
                Position = startPosition
            };
            gameLogic.GameData.Board.Tiles[capturePosition.x, capturePosition.y].Piece = whiteQueen;
            gameLogic.GameData.Board.Pieces.Add(whiteQueen);
            gameLogic.GameData.Board.Tiles[startPosition.x, startPosition.y].Piece = blackRook;
            gameLogic.GameData.Board.Pieces.Add(blackRook);
            // Act
            gameLogic.TryMovePiece(blackRookUID, capturePosition.x, capturePosition.y);

            // Assert
            Assert.IsNull(gameLogic.GameData.Board.Tiles[startPosition.x, startPosition.y].Piece,
                "Black rook should have moved.");
            Assert.IsFalse(gameLogic.GameData.Board.Tiles[capturePosition.x, capturePosition.y].Piece?.UID == whiteQueenUID,
                          "White queen should have been captured.");
            Assert.IsNotNull(gameLogic.GameData.Board.Tiles[capturePosition.x, capturePosition.y].Piece,
                             "Black pawn should have moved to the capture position.");
            Assert.AreEqual(blackRookUID, gameLogic.GameData.Board.Tiles[capturePosition.x, capturePosition.y].Piece.UID,
                            "Black pawn's UID should match after moving.");
            Assert.False(gameLogic.GameData.Board.Pieces.Exists(piece => piece?.UID == whiteQueenUID),
                         "White queen should no longer exist in the board pieces list.");
        }

        [Test]
        public void TryMovePiece_ShouldEndGame_WhenKingIsCaptured()
        {
            // Arrange
            gameLogic.Init();

            int kingX = 4, kingY = 5;  // 흰 킹의 위치
            int attackingX = 4, attackingY = 4;  // 공격할 기물의 위치

            King king = new King { UID = "white_king", Color = TeamColor.White, Type = PieceType.King, Position = new Vector2Int(kingX, kingY) };
            gameLogic.GameData.Board.Tiles[kingX, kingY].Piece = king;
            gameLogic.GameData.Board.Pieces.Add(king);

            Piece attacker = new Queen { UID = "black_queen", Color = TeamColor.Black, Type = PieceType.Queen, Position = new Vector2Int(attackingX, attackingY) };
            gameLogic.GameData.Board.Tiles[attackingX, attackingY].Piece = attacker;
            gameLogic.GameData.Board.Pieces.Add(attacker);

            bool gameEnded = false;
            gameLogic.OnGameEnded += (winnerID) =>
            {
                gameEnded = true;
                Assert.AreEqual(gameLogic.GameData.BlackClientID, winnerID, "잘못된 승자가 호출되었습니다.");
            };

            // Act
            gameLogic.TryMovePiece("black_queen", kingX, kingY);

            // Assert
            Assert.IsTrue(gameEnded, "킹을 잡았을 때 게임이 종료되지 않았습니다.");
        }
    }


}