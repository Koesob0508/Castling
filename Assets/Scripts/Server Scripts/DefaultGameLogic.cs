using Castling.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace Castling.Server
{
    public class DefaultGameLogic : IGameLogic
    {
        public GameData GameData { get; private set; }

        // 생성자: 기본 체스판 초기화
        public DefaultGameLogic(ulong blackClientID, ulong whiteClientID)
        {
            GameData = new GameData
            {
                BlackClientID = blackClientID,
                WhiteClientID = whiteClientID,
                Board = InitializeBoard()
            };
        }

        // 체스판 초기화 메서드
        private Board InitializeBoard()
        {
            int boardSize = 8;
            Board board = new Board
            {
                xSize = boardSize,
                ySize = boardSize,
                tiles = new Tile[boardSize, boardSize],
                pieces = new List<Piece>()
            };

            // 보드 타일 초기화
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    board.tiles[i, j] = new Tile
                    {
                        Position = new Vector2Int(i, j),
                        Piece = null  // 초기에는 모든 타일에 말이 없음
                    };
                }
            }

            // 말 초기화
            InitializePieces(board);

            return board;
        }

        // 기본 말 배치
        private void InitializePieces(Board board)
        {
            // 폰 (2열, 7열)
            for (int i = 0; i < 8; i++)
            {
                board.pieces.Add(new Piece { Type = PieceType.Pawn, UID = $"black_pawn_{i}", Position = new Vector2Int(i, 1) });
                board.pieces.Add(new Piece { Type = PieceType.Pawn, UID = $"white_pawn_{i}", Position = new Vector2Int(i, 6) });
            }

            // 주요 기물 (양 끝줄)
            PlacePiece(board, PieceType.Rook, 0, 0, "black_rook_0");
            PlacePiece(board, PieceType.Rook, 7, 0, "black_rook_1");
            PlacePiece(board, PieceType.Knight, 1, 0, "black_knight_0");
            PlacePiece(board, PieceType.Knight, 6, 0, "black_knight_1");
            PlacePiece(board, PieceType.Bishop, 2, 0, "black_bishop_0");
            PlacePiece(board, PieceType.Bishop, 5, 0, "black_bishop_1");
            PlacePiece(board, PieceType.Queen, 3, 0, "black_queen");
            PlacePiece(board, PieceType.King, 4, 0, "black_king");

            PlacePiece(board, PieceType.Rook, 0, 7, "white_rook_0");
            PlacePiece(board, PieceType.Rook, 7, 7, "white_rook_1");
            PlacePiece(board, PieceType.Knight, 1, 7, "white_knight_0");
            PlacePiece(board, PieceType.Knight, 6, 7, "white_knight_1");
            PlacePiece(board, PieceType.Bishop, 2, 7, "white_bishop_0");
            PlacePiece(board, PieceType.Bishop, 5, 7, "white_bishop_1");
            PlacePiece(board, PieceType.Queen, 3, 7, "white_queen");
            PlacePiece(board, PieceType.King, 4, 7, "white_king");
        }

        // 말 배치 메서드
        private void PlacePiece(Board board, PieceType type, int x, int y, string uid)
        {
            Piece piece = new Piece { Type = type, UID = uid, Position = new Vector2Int(x, y) };
            board.pieces.Add(piece);
            board.tiles[x, y].Piece = piece;
        }

        // 기물 이동 메서드
        public bool MovePiece(Vector2Int from, Vector2Int to)
        {
            Tile fromTile = GameData.Board.tiles[from.x, from.y];
            Tile toTile = GameData.Board.tiles[to.x, to.y];

            // 기물이 있는지 확인
            if (fromTile.Piece == null)
            {
                Debug.LogError("No piece at the starting position!");
                return false;
            }

            Piece pieceToMove = fromTile.Piece;

            // 움직일 수 있는 위치인지 확인 (여기서는 기초적인 이동만 구현)
            if (!IsMoveValid(pieceToMove, from, to))
            {
                Debug.LogError("Invalid move!");
                return false;
            }

            // 이동 처리
            toTile.Piece = pieceToMove;
            fromTile.Piece = null;
            pieceToMove.Position = to;

            Debug.Log($"Moved {pieceToMove.Type} to {to}");
            return true;
        }

        // 이동 가능성 검사 메서드 (기초 구현)
        private bool IsMoveValid(Piece piece, Vector2Int from, Vector2Int to)
        {
            List<Vector2Int> validMoves = piece.GetMoveableTiles(from.x, from.y).ConvertAll(tile => tile.Position);
            return validMoves.Contains(to);
        }
    }
}