using Castling.Shared;
using UnityEngine;

public class TileEntity : MonoBehaviour
{
    private int x;
    private int y;

    public int X { get => x; private set => x = value; }
    public int Y { get => y; private set => y = value; }

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }


}