using Castling.Shared;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Server
{
    public class DefaultGameSession : IGameSession
    {
        private List<PlayerInfo> Players;
        private GameData GameData;

        public void Init(List<PlayerInfo> playerInfos)
        {
            Debug.Log("Game session initilaize");

            Players = playerInfos;

            GameData = new();
            GameData.BlackClientID = Players[0].ClientID;
            GameData.WhiteClientID = Players[0].ClientID;

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("FromClient", OnReceivedClientMessage);

            SendStartGame();
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

        #region Send

        private void SendStartGame()
        {
            FastBufferWriter writer = new FastBufferWriter(size: 128, allocator: Unity.Collections.Allocator.Temp);

            writer.WriteValueSafe(CommandType.StartGame);
            writer.WriteValueSafe(GameData);

            foreach (var player in Players)
            {
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("FromServer", player.ClientID, writer, NetworkDelivery.ReliableSequenced);
            }
        }

        #endregion
    }


}