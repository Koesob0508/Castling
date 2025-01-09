using Castling.Shared;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Castling.Server
{
    public class DefaultGameSession : IGameSession
    {
        public int SessionID { get; private set; }
        private List<PlayerInfo> Players;
        private IGameLogic Logic;

        public void Init(int sessionID, List<PlayerInfo> playerInfos)
        {
            Debug.Log("Game session initilaize");

            SessionID = sessionID;
            Players = playerInfos;

            Logic = new DefaultGameLogic(Players[0].ClientID, Players[1].ClientID);

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("FromClient", OnReceivedClientMessage);

            SendStartGame();
        }

        public void Clear()
        {
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("FromClient");
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
            }
        }

        private void TryMovePiece(string pieceUID, int x, int y)
        {

        }

        #region Send

        private void SendStartGame()
        {
            Debug.Log("Start Game");

            using (FastBufferWriter writer = new FastBufferWriter(size: 4096, allocator: Unity.Collections.Allocator.Temp))
            {
                writer.WriteValueSafe(CommandType.StartGame);
                writer.WriteValueSafe(Logic.GameData);

                SendToAll(writer);
            }
        }

        private void SendToAll(FastBufferWriter writer)
        {
            foreach(var player in Players)
            {
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("FromServer", player.ClientID, writer, NetworkDelivery.ReliableSequenced);
            }
        }

        #endregion
    }


}