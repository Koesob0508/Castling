using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Tests
{
    [TestFixture]
    public class BoardTest
    {
        [Test]
        public void Board_SerializeDeserialize_Test()
        {
            // 테스트용 Board 생성
            Board originalBoard = new Board
            {
                xSize = 8,
                ySize = 8,
                tiles = new Tile[8, 8],
                pieces = new List<Piece>
        {
            new Piece { Type = PieceType.King, UID = "king_1", Position = new Vector2Int(0, 0) },
            new Piece { Type = PieceType.Queen, UID = "queen_1", Position = new Vector2Int(1, 1) }
        }
            };

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    originalBoard.tiles[i, j] = new Tile { Position = new Vector2Int(i, j) };
                }
            }

            // Serialize
            using (FastBufferWriter writer = new FastBufferWriter(2048, Allocator.Temp))
            {
                writer.WriteNetworkSerializable(originalBoard);

                // Deserialize
                using (FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp))
                {
                    Board deserializedBoard = new Board();
                    reader.ReadNetworkSerializable(out deserializedBoard);

                    // Assert 검증
                    Assert.AreEqual(originalBoard.xSize, deserializedBoard.xSize);
                    Assert.AreEqual(originalBoard.ySize, deserializedBoard.ySize);
                    Assert.AreEqual(originalBoard.pieces.Count, deserializedBoard.pieces.Count);

                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Assert.AreEqual(originalBoard.tiles[i, j].Position, deserializedBoard.tiles[i, j].Position);
                        }
                    }

                    for (int i = 0; i < originalBoard.pieces.Count; i++)
                    {
                        Assert.AreEqual(originalBoard.pieces[i].Type, deserializedBoard.pieces[i].Type);
                        Assert.AreEqual(originalBoard.pieces[i].UID, deserializedBoard.pieces[i].UID);
                        Assert.AreEqual(originalBoard.pieces[i].Position, deserializedBoard.pieces[i].Position);
                    }
                }
            }
        }
    }
}