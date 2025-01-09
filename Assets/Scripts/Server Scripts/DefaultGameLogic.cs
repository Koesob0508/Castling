using Castling.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Castling.Server
{
    public class DefaultGameLogic : IGameLogic
    {
        public GameData GameData
        {
            get; private set;
        }

        public event Action OnGameStarted;
        public event Action<ulong> OnGameEnded;
        public event Action OnTurnChanged;
        public event Action OnMoveFailed;
        public event Action OnMoveSucceeded;

        // 생성자: 기본 체스판 초기화
        public DefaultGameLogic(ulong blackClientID, ulong whiteClientID)
        {
            GameData = new GameData
            {
                BlackClientID = blackClientID,
                WhiteClientID = whiteClientID,
                CurrentClientID = blackClientID,
            };
        }

        public void Init()
        {
            Debug.Log("GameLogic Init");
            GameData.Board = InitializeBoard();
            OnGameStarted?.Invoke();
        }

        public void Clear()
        {
            // 보드 및 기물 데이터 초기화
            GameData.Board.Tiles = null;
            GameData.Board.Pieces.Clear();
            GameData = null;

            // 이벤트 핸들러 해제
            OnGameStarted = null;
            OnGameEnded = null;
            OnTurnChanged = null;
            OnMoveFailed = null;
            OnMoveSucceeded = null;

            Debug.Log("GameData and events cleared.");
        }

        // 체스판 초기화 메서드
        private Board InitializeBoard()
        {
            int boardSize = 8;
            Board board = new Board
            {
                xSize = boardSize,
                ySize = boardSize,
                Tiles = new Tile[boardSize, boardSize],
                Pieces = new List<Piece>()
            };

            // 보드 타일 초기화
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    board.Tiles[i, j] = new Tile
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
        private readonly PieceType[] majorPieces = { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen, PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };

        // 기본 말 배치
        private void InitializePieces(Board board)
        {
            // 폰 (2열, 7열)
            for (int i = 0; i < 8; i++)
            {
                PlacePiece(board, TeamColor.Black, PieceType.Pawn, i, 1, $"black_pawn_{i}");
                PlacePiece(board, TeamColor.White, PieceType.Pawn, i, 6, $"white_pawn_{i}");

                PlacePiece(board, TeamColor.Black, majorPieces[i], i, 0, $"black_{majorPieces[i].ToString().ToLower()}_{i}");
                PlacePiece(board, TeamColor.White, majorPieces[i], i, 7, $"white_{majorPieces[i].ToString().ToLower()}_{i}");
            }
        }

        // 말 배치 메서드
        private void PlacePiece(Board board, TeamColor color, PieceType type, int x, int y, string uid)
        {
            Piece piece = null;
            switch (type)
            {
                case PieceType.None:
                    Debug.LogWarning("None type piece found.");
                    break;
                case PieceType.King:
                    piece = new King { Color = color, Type = type, UID = uid, Position = new Vector2Int(x, y) };
                    break;
                case PieceType.Queen:
                    piece = new Queen { Color = color, Type = type, UID = uid, Position = new Vector2Int(x, y) };
                    break;
                case PieceType.Bishop:
                    piece = new Bishop { Color = color, Type = type, UID = uid, Position = new Vector2Int(x, y) };
                    break;
                case PieceType.Knight:
                    piece = new Knight { Color = color, Type = type, UID = uid, Position = new Vector2Int(x, y) };
                    break;
                case PieceType.Rook:
                    piece = new Rook { Color = color, Type = type, UID = uid, Position = new Vector2Int(x, y) };
                    break;
                case PieceType.Pawn:
                    piece = new Pawn { Color = color, Type = type, UID = uid, Position = new Vector2Int(x, y) };
                    break;
            }

            board.Pieces.Add(piece);
            board.Tiles[x, y].Piece = piece;
        }

        public void TryMovePiece(string pieceUID, int destinationX, int destinationY)
        {
            // pieceUID를 통해 해당 piece를 찾는다.
            Piece pieceToMove = GameData.Board.Pieces.Find(piece => piece.UID == pieceUID);

            if (pieceToMove == null)
            {
                Debug.LogError($"No piece found with UID: {pieceUID}");
                OnMoveFailed?.Invoke();
                return;
            }

            // 해당 piece가 움직일 수 있는 턴인지 확인
            if (pieceToMove.Color != GetTeamColor(GameData.CurrentClientID))
            {
                Debug.LogError("It's not this player's turn!");
                OnMoveFailed?.Invoke();
                return;
            }

            // 현재 위치와 목표 위치 설정
            Vector2Int from = pieceToMove.Position;
            Vector2Int to = new Vector2Int(destinationX, destinationY);

            // MovePiece 호출하여 이동 시도
            if (!MovePiece(from, to))
            {
                Debug.LogError("Move failed!");
                OnMoveFailed?.Invoke();
                return;
            }

            OnMoveSucceeded?.Invoke();

            // 턴 교체
            ChangeTurn();
        }

        // 기물 이동 메서드
        private bool MovePiece(Vector2Int from, Vector2Int to)
        {
            Tile fromTile = GameData.Board.Tiles[from.x, from.y];
            Tile toTile = GameData.Board.Tiles[to.x, to.y];

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
            List<Vector2Int> validMoves = piece.GetMoveableTiles(GameData, from.x, from.y).ConvertAll(tile => tile.Position);
            return validMoves.Contains(to);
        }

        // 턴 교체 메서드
        private void ChangeTurn()
        {
            GameData.CurrentClientID = GameData.CurrentClientID == GameData.BlackClientID ? GameData.WhiteClientID : GameData.BlackClientID;
            Debug.Log($"It's now {GetTeamColor(GameData.CurrentClientID)}'s turn.");

            OnTurnChanged?.Invoke();
        }

        private TeamColor GetTeamColor(ulong clientID)
        {
            if (clientID == GameData.BlackClientID)
            {
                return TeamColor.Black;
            }
            else // if (clientID == GameData.WhiteClientID)
            {
                return TeamColor.White;
            }
        }
    }
}