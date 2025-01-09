using Castling.Shared;
using UnityEngine;

public class TileEntity : MonoBehaviour
{
    private Vector2Int position = new Vector2Int();
    [SerializeField]
    private GameObject MoveableEffect;

    public Vector2Int Position { get => position; set => position = value; }

    public void Init(int x, int y)
    {
        position.x = x;
        position.y = y;
        MoveableEffect.SetActive(false);
    }

    public void ShowMoveableEffect()
    {
        MoveableEffect.SetActive(true);
    }

    public void HideMoveableEffect()
    {
        MoveableEffect.SetActive(false);
    }
}