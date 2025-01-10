using Castling.Server;
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

    public enum TeamColor
    {
        Black = 0,
        White = 1,
    }

    public class Piece : INetworkSerializable
    {
        public TeamColor Color;
        public PieceType Type = PieceType.None;
        public string UID;
        public Vector2Int Position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int colorValue = (int)Color;
            serializer.SerializeValue(ref colorValue);
            int enumValue = (int)Type;
            serializer.SerializeValue(ref enumValue);

            if (!serializer.IsWriter)
            {
                Color = (TeamColor)colorValue;
                Type = (PieceType)enumValue;
            }

            serializer.SerializeValue(ref UID);
            serializer.SerializeValue(ref Position);
        }

        public virtual List<Tile> GetMoveableTiles(GameData gameData, int currentX, int currentY) { return null; }
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
        public Tile[,] Tiles;
        public List<Piece> Pieces;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref xSize);
            serializer.SerializeValue(ref ySize);

            if (serializer.IsWriter)
            {
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < ySize; j++)
                    {
                        Tile tile = Tiles[i, j];
                        serializer.SerializeNetworkSerializable(ref tile);
                    }
                }
            }
            else
            {
                Tiles = new Tile[xSize, ySize];
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < ySize; j++)
                    {
                        Tile tile = new Tile();  // �� Tile �ν��Ͻ� ����
                        serializer.SerializeNetworkSerializable(ref tile);
                        Tiles[i, j] = tile;
                    }
                }
            }

            int piecesCount = Pieces?.Count ?? 0;
            serializer.SerializeValue(ref piecesCount);

            if (serializer.IsWriter)
            {
                foreach (Piece piece in Pieces)
                {
                    Piece tempPiece = piece;
                    serializer.SerializeNetworkSerializable(ref tempPiece);
                }
            }
            else
            {
                Pieces = new List<Piece>(piecesCount);
                for (int i = 0; i < piecesCount; i++)
                {
                    Piece piece = new Piece();
                    serializer.SerializeNetworkSerializable(ref piece);
                    Pieces.Add(piece);
                }
            }
        }
    }

    public class GameData : INetworkSerializable
    {
        public ulong BlackClientID;
        public ulong WhiteClientID;
        public Board Board;
        public ulong CurrentClientID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref BlackClientID);
            serializer.SerializeValue(ref WhiteClientID);
            serializer.SerializeValue(ref CurrentClientID);

            bool hasBoard = Board != null;
            serializer.SerializeValue(ref hasBoard);

            if (hasBoard)
            {
                if (!serializer.IsWriter && Board == null)
                {
                    Board = new Board();
                }

                serializer.SerializeNetworkSerializable(ref Board);
            }
        }
    }
}