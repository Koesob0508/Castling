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
        public Vector2Int Position;

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
            serializer.SerializeValue(ref Position);
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

            serializer.SerializeValue(ref Position);
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
            // 1. Board 크기 직렬화
            serializer.SerializeValue(ref xSize);
            serializer.SerializeValue(ref ySize);

            // 2. Tile 배열 직렬화
            if (serializer.IsWriter)
            {
                // Writer일 경우, 배열 직렬화
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < ySize; j++)
                    {
                        Tile tile = tiles[i, j];
                        serializer.SerializeValue(ref tile);
                    }
                }
            }
            else
            {
                // Reader일 경우, 배열 초기화 후 역직렬화
                tiles = new Tile[xSize, ySize];
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < ySize; j++)
                    {
                        Tile tile = new Tile();  // 새 Tile 인스턴스 생성
                        serializer.SerializeValue(ref tile);
                        tiles[i, j] = tile;
                    }
                }
            }

            // 3. Piece 리스트 직렬화
            int piecesCount = pieces?.Count ?? 0;
            serializer.SerializeValue(ref piecesCount);

            if (serializer.IsWriter)
            {
                // Writer일 경우, 리스트 직렬화
                foreach (Piece piece in pieces)
                {
                    Piece tempPiece = piece;
                    serializer.SerializeValue(ref tempPiece);
                }
            }
            else
            {
                // Reader일 경우, 리스트 초기화 후 역직렬화
                pieces = new List<Piece>(piecesCount);
                for (int i = 0; i < piecesCount; i++)
                {
                    Piece piece = new Piece();  // 새 Piece 인스턴스 생성
                    serializer.SerializeValue(ref piece);
                    pieces.Add(piece);
                }
            }
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