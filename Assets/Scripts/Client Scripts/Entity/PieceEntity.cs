using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PieceEntity : MonoBehaviour
{
    private PieceType pieceID = 0;
    private string uid = string.Empty;
    private eColor color;
    private int x;
    private int y;

    public PieceType PieceID { get => pieceID; set => pieceID = value; }
    public string UID { get => uid; set => uid = value; }
    public eColor Color { get => color; set => color = value; }
    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }

}

public enum eColor
{
    Black = 0,
    White = 1,
}