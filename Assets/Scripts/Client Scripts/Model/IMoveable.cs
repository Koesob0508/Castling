using Castling.Shared;
using System.Collections.Generic;

public interface IMoveable
{
    public List<Tile> GetMoveableTiles();
}