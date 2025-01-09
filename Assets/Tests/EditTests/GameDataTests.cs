using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace Castling.Tests
{
    public class GameDataTests
    {
        [Test]
        public void GameData_SerializeDeserialize_Test()
        {
            // 1. 원본 GameData 객체 생성
            GameData originalGameData = new GameData
            {
                BlackClientID = 123456789,
                WhiteClientID = 987654321,
                Board = new Board
                {
                    xSize = 8,
                    ySize = 8,
                    tiles = new Tile[8, 8],
                    pieces = new List<Piece>
                {
                    new Piece { Type = PieceType.King},
                    new Piece { Type = PieceType.Queen}
                }
                }
            };

            // 2. tiles 배열 초기화
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    originalGameData.Board.tiles[i, j] = new Tile { Position = UnityEngine.Vector2Int.down };
                }
            }

            // 3. FastBufferWriter를 사용하여 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp))
            {
                writer.WriteNetworkSerializable(originalGameData);

                // 4. FastBufferReader를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp))
                {
                    GameData deserializedGameData = new GameData();
                    reader.ReadNetworkSerializable(out deserializedGameData);

                    // 5. 값 검증
                    Assert.AreEqual(originalGameData.BlackClientID, deserializedGameData.BlackClientID, "BlackClientID mismatch");
                    Assert.AreEqual(originalGameData.WhiteClientID, deserializedGameData.WhiteClientID, "WhiteClientID mismatch");
                    Assert.AreEqual(originalGameData.Board.xSize, deserializedGameData.Board.xSize, "Board xSize mismatch");
                    Assert.AreEqual(originalGameData.Board.ySize, deserializedGameData.Board.ySize, "Board ySize mismatch");
                    Assert.AreEqual(originalGameData.Board.pieces.Count, deserializedGameData.Board.pieces.Count, "Pieces count mismatch");

                    for (int i = 0; i < originalGameData.Board.pieces.Count; i++)
                    {
                        Assert.AreEqual(originalGameData.Board.pieces[i].Type, deserializedGameData.Board.pieces[i].Type, $"Piece {i} type mismatch");
                    }

                    for (int i = 0; i < originalGameData.Board.xSize; i++)
                    {
                        for (int j = 0; j < originalGameData.Board.ySize; j++)
                        {
                            Assert.AreEqual(originalGameData.Board.tiles[i, j].Position.x, deserializedGameData.Board.tiles[i, j].Position.x, $"Tile [{i},{j}] x mismatch");
                            Assert.AreEqual(originalGameData.Board.tiles[i, j].Position.y, deserializedGameData.Board.tiles[i, j].Position.y, $"Tile [{i},{j}] y mismatch");
                        }
                    }
                }
            }
        }
    }
}