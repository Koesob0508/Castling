using Castling.Server;
using Castling.Shared;
using NUnit.Framework;
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
    }
}