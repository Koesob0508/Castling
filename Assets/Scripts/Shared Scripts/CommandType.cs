namespace Castling.Shared
{
    public static class CommandType
    {
        public const ushort None = 0;

        // Client To Server
        public const ushort TryMovePiece = 1000;

        // Server To Client
        public const ushort StartGame = 2000;
        public const ushort EndGame = 2010;
        public const ushort ChangeTurn = 2020;
        public const ushort MovePieceResult = 2050;
    }
}