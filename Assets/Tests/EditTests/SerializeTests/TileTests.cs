using Castling.Shared;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Tests
{
    [TestFixture]
    public class TileTests
    {
        [Test]
        public void Tile_SerializeDeserialize_Test_WithPiece()
        {
            // 1. 원본 Tile 객체 생성
            Tile originalTile = new Tile
            {
                Position = new Vector2Int(4, 7),
                Piece = new Piece
                {
                    Type = PieceType.Queen,
                    UID = "piece_456",
                    Position = new Vector2Int(4, 7)
                }
            };

            // 2. FastBufferWriter를 사용하여 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp))
            {
                writer.WriteNetworkSerializable(originalTile);

                // 3. FastBufferReader를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp))
                {
                    Tile deserializedTile = new Tile();
                    reader.ReadNetworkSerializable(out deserializedTile);

                    // 4. 검증
                    Assert.AreEqual(originalTile.Position, deserializedTile.Position, "Position mismatch");
                    Assert.IsNotNull(deserializedTile.Piece, "Piece should not be null");
                    Assert.AreEqual(originalTile.Piece.Type, deserializedTile.Piece.Type, "PieceType mismatch");
                    Assert.AreEqual(originalTile.Piece.UID, deserializedTile.Piece.UID, "UID mismatch");
                    Assert.AreEqual(originalTile.Piece.Position, deserializedTile.Piece.Position, "Piece Position mismatch");
                }
            }
        }

        [Test]
        public void Tile_SerializeDeserialize_Test_WithoutPiece()
        {
            // 1. 원본 Tile 객체 생성 (Piece가 null)
            Tile originalTile = new Tile
            {
                Position = new Vector2Int(3, 2),
                Piece = null
            };

            // 2. FastBufferWriter를 사용하여 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp))
            {
                writer.WriteNetworkSerializable(originalTile);

                // 3. FastBufferReader를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp))
                {
                    Tile deserializedTile = new Tile();
                    reader.ReadNetworkSerializable(out deserializedTile);

                    // 4. 검증
                    Assert.AreEqual(originalTile.Position, deserializedTile.Position, "Position mismatch");
                    Assert.IsNull(deserializedTile.Piece, "Piece should be null");
                }
            }
        }
    }
}