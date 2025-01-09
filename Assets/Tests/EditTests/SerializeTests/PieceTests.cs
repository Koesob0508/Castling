using Castling.Shared;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Tests
{
    [TestFixture]
    public class PieceTests
    {
        [Test]
        public void Piece_SerializeDeserialize_Test()
        {
            // 1. 원본 Piece 객체 생성
            Piece originalPiece = new Piece
            {
                Color = TeamColor.White,
                Type = PieceType.King,
                UID = "piece_123",
                Position = new Vector2Int(3, 5)
            };

            // 2. FastBufferWriter를 사용하여 직렬화
            using (FastBufferWriter writer = new FastBufferWriter(1024, Allocator.Temp))
            {
                writer.WriteNetworkSerializable(originalPiece);

                // 3. FastBufferReader를 사용하여 역직렬화
                using (FastBufferReader reader = new FastBufferReader(writer, Allocator.Temp))
                {
                    Piece deserializedPiece = new Piece();
                    reader.ReadNetworkSerializable(out deserializedPiece);

                    // 4. 검증
                    Assert.AreEqual(originalPiece.Color, deserializedPiece.Color, "PieceColor mismatch");
                    Assert.AreEqual(originalPiece.Type, deserializedPiece.Type, "PieceType mismatch");
                    Assert.AreEqual(originalPiece.UID, deserializedPiece.UID, "UID mismatch");
                    Assert.AreEqual(originalPiece.Position, deserializedPiece.Position, "Position mismatch");
                }
            }
        }
    }
}