using System;
using Castling.Shared;

namespace Castling.Server
{
    public interface IGameLogic
    {
        GameData GameData { get; }

        event Action OnGameStarted;
        event Action<ulong> OnGameEnded;
        event Action OnTurnChanged;
        event Action OnMoveFailed;
        event Action OnMoveSucceeded;

        void Init();
        void Clear();
        void TryMovePiece(string pieceUID, int destinationX, int destinationY);
    }
}