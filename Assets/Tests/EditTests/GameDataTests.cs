using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Tests
{
    public class GameDataTests
    {
        [Test]
        public void GameData_SerializeDeserialize_Test()
        {
            // 1. 테스트용 GameData 생성
            GameData originalGameData = new GameData
            {
                BlackClientID = 12345,
                WhiteClientID = 67890,
                Board = new Board
                {
                    xSize = 8,
                    ySize = 8,
                    Tiles = new Tile[8, 8],
                    Pieces = new List<Piece>
            {
                new Piece { Type = PieceType.King, UID = "king_1", Position = new Vector2Int(0, 0) },
                new Piece { Type = PieceType.Queen, UID = "queen_1", Position = new Vector2Int(1, 1) }
            }
                }
            };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    originalGameData.Board.Tiles[i, j] = new Tile { Position = new Vector2Int(i, j) };
                }
            }

            // 2. 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(4096, Allocator.Temp))
            {
                writer.WriteNetworkSerializable(originalGameData);

                // 3. 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp))
                {
                    GameData deserializedGameData = new GameData();
                    reader.ReadNetworkSerializable(out deserializedGameData);

                    // 4. 검증
                    Assert.AreEqual(originalGameData.BlackClientID, deserializedGameData.BlackClientID, "BlackClientID mismatch");
                    Assert.AreEqual(originalGameData.WhiteClientID, deserializedGameData.WhiteClientID, "WhiteClientID mismatch");

                    Assert.IsNotNull(deserializedGameData.Board, "Board should not be null");
                    Assert.AreEqual(originalGameData.Board.xSize, deserializedGameData.Board.xSize, "Board xSize mismatch");
                    Assert.AreEqual(originalGameData.Board.ySize, deserializedGameData.Board.ySize, "Board ySize mismatch");
                    Assert.AreEqual(originalGameData.Board.Pieces.Count, deserializedGameData.Board.Pieces.Count, "Pieces count mismatch");

                    for (int i = 0; i < originalGameData.Board.Pieces.Count; i++)
                    {
                        Assert.AreEqual(originalGameData.Board.Pieces[i].Type, deserializedGameData.Board.Pieces[i].Type, $"Piece {i} Type mismatch");
                        Assert.AreEqual(originalGameData.Board.Pieces[i].UID, deserializedGameData.Board.Pieces[i].UID, $"Piece {i} UID mismatch");
                        Assert.AreEqual(originalGameData.Board.Pieces[i].Position, deserializedGameData.Board.Pieces[i].Position, $"Piece {i} Position mismatch");
                    }
                }
            }
        }
    }
}