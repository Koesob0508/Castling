using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace Castling.Tests
{
    public class GameDataTests
    {
        private GameData CreateSampleGameData()
        {
            GameData gameData = new();

            for (int i = 0; i < 8; i++)
            {
                gameData.Board.Add(new List<Tile>());
                for (int j = 0; j < 8; j++)
                {
                    var pieceType = (i == 0 && (j == 0 || j == 7)) ? PieceType.Rook : PieceType.None;
                    gameData.Board[i].Add(new Tile
                    {
                        Piece = pieceType != PieceType.None ? new Piece
                        {
                            Type = pieceType,
                            UID = $"{i}-{j}-UID"
                        } : null
                    });
                }
            }

            return gameData;
        }

        [Test]
        public void GameData_NetworkSerialize_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            GameData originalGameData = CreateSampleGameData();

            // 1. FastBufferWriter 생성(쓰기)
            using var writer = new FastBufferWriter(10240, Allocator.Temp);

            // Act - 직렬화
            writer.WriteNetworkSerializable(originalGameData);

            // 2. FastBufferReader 생성(읽기)
            using var reader = new FastBufferReader(writer.ToArray(), Allocator.Temp);

            GameData deserializedGameData = new GameData();
            reader.ReadNetworkSerializable(out deserializedGameData);

            // Assert - 직렬화 후 결과 비교
            Assert.AreEqual(originalGameData.Board.Count, deserializedGameData.Board.Count, "Board 행 개수가 다릅니다.");

            for (int i = 0; i < originalGameData.Board.Count; i++)
            {
                Assert.AreEqual(originalGameData.Board[i].Count, deserializedGameData.Board[i].Count, $"Board[{i}] 열 개수가 다릅니다.");
                for (int j = 0; j < originalGameData.Board[i].Count; j++)
                {
                    var originalTile = originalGameData.Board[i][j];
                    var deserializedTile = deserializedGameData.Board[i][j];

                    if (originalTile.Piece == null)
                    {
                        Assert.IsNull(deserializedTile.Piece, $"Board[{i}][{j}] 기물이 다릅니다. (둘 다 비어야 합니다)");
                    }
                    else
                    {
                        Assert.IsNotNull(deserializedTile.Piece, $"Board[{i}][{j}] 기물이 다릅니다. (기물이 있어야 합니다)");
                        Assert.AreEqual(originalTile.Piece.Type, deserializedTile.Piece.Type, $"Board[{i}][{j}] 기물의 Type이 다릅니다.");
                        Assert.AreEqual(originalTile.Piece.UID, deserializedTile.Piece.UID, $"Board[{i}][{j}] 기물의 UID가 다릅니다.");
                    }
                }
            }
        }
    }
}