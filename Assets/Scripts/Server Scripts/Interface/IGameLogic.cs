using Castling.Shared;

namespace Castling.Server
{
    public interface IGameLogic
    {
        GameData GameData { get; }

        bool TryMovePiece(string pieceUID, int destinationX, int destinationY);
    }
}