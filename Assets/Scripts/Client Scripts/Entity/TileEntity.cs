using Castling.Shared;
using UnityEngine;

public class TileEntity : MonoBehaviour
{
    private Vector2Int position = new Vector2Int();
    [SerializeField]
    private GameObject MoveableEffect;
    private bool isMoveable = false;
    public Vector2Int Position { get => position; set => position = value; }
    public bool IsMoveable => isMoveable;

    public void Init(int x, int y)
    {
        position.x = x;
        position.y = y;
        MoveableEffect.SetActive(false);
    }

    public void ShowMoveableEffect()
    {
        MoveableEffect.SetActive(true);
        isMoveable = true;
    }

    public void HideMoveableEffect()
    {
        MoveableEffect.SetActive(false);
        isMoveable= false;
    }
}