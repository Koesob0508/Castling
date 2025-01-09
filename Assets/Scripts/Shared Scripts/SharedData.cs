using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Shared
{
    public enum PieceType
    {
        None = 0,
        King = 1,
        Queen = 2,
        Bishop = 3,
        Knight = 4,
        Rook = 5,
        Pawn = 6
    }

    public class Piece : INetworkSerializable
    {
        public PieceType Type = PieceType.None;
        public string UID;
        public Tile Tile;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // `PieceType` enum을 `int`로 직렬화
            int enumValue = (int)Type;
            serializer.SerializeValue(ref enumValue);

            // 역직렬화 시 `enum`으로 변환
            if (!serializer.IsWriter)
            {
                Type = (PieceType)enumValue;
            }

            serializer.SerializeValue(ref UID);
        }

        public virtual List<Tile> GetMoveableTiles(int currentX, int currentY) { return null; }
    }

    public class Tile : INetworkSerializable
    {
        public Vector2Int Position;
        public Piece? Piece;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            bool hasPiece = Piece != null;
            serializer.SerializeValue(ref hasPiece);

            if (hasPiece)
            {
                if (!serializer.IsWriter && Piece == null)
                {
                    Piece = new Piece();
                }

                Piece?.NetworkSerialize(serializer);
            }
        }
    }

    public class Board : INetworkSerializable
    {
        public int xSize;
        public int ySize;
        public Tile[,] tiles;
        public List<Piece> pieces;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            throw new NotImplementedException();
        }
    }

    public class GameData : INetworkSerializable
    {
        public ulong BlackClientID;
        public ulong WhiteClientID;
        public Board Board;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
        //    // 0. BlackClientID와 WhiteClientID 직렬화
        //    serializer.SerializeValue(ref BlackClientID);
        //    serializer.SerializeValue(ref WhiteClientID);

        //    // 1. 리스트의 외부 크기(행 수) 직렬화
        //    int outerCount = Board.Count;
        //    serializer.SerializeValue(ref outerCount);

        //    // 2. 직렬화 중일 때는 Board 초기화 필요 (Deserialize 과정)
        //    if (serializer.IsReader)
        //    {
        //        Board = new List<List<Tile>>(outerCount);
        //    }

        //    for (int i = 0; i < outerCount; i++)
        //    {
        //        // 3. 내부 리스트가 null 일 경우 초기화
        //        if (serializer.IsReader && Board.Count <= i)
        //        {
        //            Board.Add(new List<Tile>());
        //        }

        //        // 4. 내부 리스트(열)의 크기 직렬화
        //        int innerCount = serializer.IsWriter ? Board[i].Count : 0;
        //        serializer.SerializeValue(ref innerCount);

        //        // 5. 내부 리스트 요소 직렬화
        //        if (serializer.IsReader)
        //        {
        //            Board[i] = new List<Tile>(innerCount);
        //        }

        //        for (int j = 0; j < innerCount; j++)
        //        {
        //            Tile value = serializer.IsWriter ? Board[i][j] : new Tile();
        //            serializer.SerializeValue(ref value);

        //            if (serializer.IsReader)
        //            {
        //                Board[i].Add(value);
        //            }
        //        }

        //    }
        }
    }
}