using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Castling.Tests.PieceMove
{
    [TestFixture]
    public class PawnMovetTests
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
        public void TryMovePiece_BlackPawn_ValidMove_OneStepForward_ShouldSucceed()
        {
            // Arrange
            gameLogic.Init();

            string pieceUID = "black_pawn_0"; // 좌측 흑 폰의 UID
            int startX = 0, startY = 1; // 기본 위치
            int destinationX = 0, destinationY = 2; // 1칸 전진 위치

            // Act
            bool moveSucceeded = false;
            gameLogic.OnMoveSucceeded += () => moveSucceeded = true;

            gameLogic.TryMovePiece(pieceUID, destinationX, destinationY);

            // Assert
            Assert.IsTrue(moveSucceeded, "흑 폰의 1칸 전진이 실패했습니다.");
            Piece movedPiece = gameLogic.GameData.Board.Tiles[destinationX, destinationY].Piece;
            Assert.IsNotNull(movedPiece, "이동한 위치에 기물이 없습니다.");
            Assert.AreEqual(pieceUID, movedPiece.UID, "움직인 기물의 UID가 일치하지 않습니다.");
            Assert.IsNull(gameLogic.GameData.Board.Tiles[startX, startY].Piece, "기물이 원래 위치에서 제거되지 않았습니다.");
        }

        [Test]
        public void TryMovePiece_BlackPawn_FirstMove_TwoStepsForward_ShouldSucceed()
        {
            // Arrange
            gameLogic.Init();

            string pieceUID = "black_pawn_1"; // 흑 폰 UID
            int startX = 1, startY = 1; // 기본 위치
            int destinationX = 1, destinationY = 3; // 첫 이동 시 2칸 전진 위치

            // Act
            GameData gameData = gameLogic.GameData;
            ulong currentClientID = gameData.BlackClientID;
            gameData.CurrentClientID = currentClientID; // 흑의 턴 설정

            bool moveSucceeded = false;
            gameLogic.OnMoveSucceeded += () => moveSucceeded = true;

            gameLogic.TryMovePiece(pieceUID, destinationX, destinationY);

            // Assert
            Assert.IsTrue(moveSucceeded, "흑 폰의 첫 2칸 전진이 실패했습니다.");
            Piece movedPiece = gameData.Board.Tiles[destinationX, destinationY].Piece;
            Assert.IsNotNull(movedPiece, "이동한 위치에 기물이 없습니다.");
            Assert.AreEqual(pieceUID, movedPiece.UID, "움직인 기물의 UID가 일치하지 않습니다.");
            Assert.IsNull(gameData.Board.Tiles[startX, startY].Piece, "기물이 원래 위치에서 제거되지 않았습니다.");
        }

        [Test]
        public void TryMovePiece_BlackPawn_InvalidMove_DiagonalWithoutCapture_ShouldFail()
        {
            // Arrange
            gameLogic.Init();

            string pieceUID = "black_pawn_2"; // 흑 폰 UID
            int startX = 2, startY = 1; // 기본 위치
            int destinationX = 3, destinationY = 2; // 대각선 1칸 이동 (공격 불가능한 자리)

            // Act
            GameData gameData = gameLogic.GameData;
            ulong currentClientID = gameData.BlackClientID;
            gameData.CurrentClientID = currentClientID; // 흑의 턴 설정

            bool moveFailed = false;
            gameLogic.OnMoveFailed += () => moveFailed = true;

            gameLogic.TryMovePiece(pieceUID, destinationX, destinationY);

            // Assert
            LogAssert.Expect(UnityEngine.LogType.Error, "Invalid move!");
            LogAssert.Expect(UnityEngine.LogType.Error, "Move failed!");
            Assert.IsTrue(moveFailed, "공격하지 않는 대각선 이동이 성공했습니다.");
            Piece originalPiece = gameData.Board.Tiles[startX, startY].Piece;
            Assert.IsNotNull(originalPiece, "원래 위치에 기물이 존재해야 합니다.");
            Assert.AreEqual(pieceUID, originalPiece.UID, "원래 위치의 기물 UID가 일치하지 않습니다.");
            Assert.IsNull(gameData.Board.Tiles[destinationX, destinationY].Piece, "잘못된 이동 위치에 기물이 없어야 합니다.");
        }

        [Test]
        public void TryMovePiece_BlackPawn_TurnCheck_ShouldFailIfNotTurn()
        {
            // Arrange
            gameLogic.Init();

            string pieceUID = "black_pawn_0"; // 흑 폰 UID
            int destinationX = 0, destinationY = 2; // 1칸 전진 위치

            // White's turn
            GameData gameData = gameLogic.GameData;
            gameData.CurrentClientID = gameData.WhiteClientID; // 백의 턴 설정

            bool moveFailed = false;
            gameLogic.OnMoveFailed += () => moveFailed = true;

            // Act
            gameLogic.TryMovePiece(pieceUID, destinationX, destinationY);

            // Assert
            LogAssert.Expect(UnityEngine.LogType.Error, "It's not this player's turn!");
            Assert.IsTrue(moveFailed, "백의 턴에 흑 폰이 움직일 수 있으면 안 됩니다.");
        }
    }
}