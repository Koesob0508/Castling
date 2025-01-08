using Castling.Shared;
using Unity.Netcode;

public class ServerManager
{
    public void Init()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("FromClient", OnReceivedClientMessage);
    }

    private void OnReceivedClientMessage(ulong clientID, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ushort commandType);

        switch (commandType)
        {
            case CommandType.TryMovePiece:
                reader.ReadValueSafe(out string uid);
                reader.ReadValueSafe(out int destinationX);
                reader.ReadValueSafe(out int destinationY);
                TryMovePiece(uid, destinationX, destinationY);
                break;
            case CommandType.StartGame:
                // Server에서는 실행하지 않음
                break;
            case CommandType.EndGame:
                // Server에서는 실행하지 않음
                break;
            case CommandType.MovePieceResult:
                // Server에서는 실행하지 않음
                break;
        }
    }

    private void TryMovePiece(string pieceUID, int x, int y)
    {

    }
}