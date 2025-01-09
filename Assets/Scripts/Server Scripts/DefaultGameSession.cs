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

        private void TryMovePiece(string pieceUID, int destinationX, int destinationY)
        {
            if (Logic.TryMovePiece(pieceUID, destinationX, destinationY))
            {
                SendMovePieceResult(true);
            }
            else
            {
                SendMovePieceResult(false);
            }
        }

        #region Send

        private void SendStartGame()
        {
            Debug.Log("Start Game");

            using (FastBufferWriter writer = new FastBufferWriter(128, allocator: Unity.Collections.Allocator.Temp, 10240))
            {
                writer.WriteValueSafe(CommandType.StartGame);
                writer.WriteNetworkSerializable(Logic.GameData);

                SendToAll(writer);
            }
        }

        private void SendMovePieceResult(bool succeededed)
        {
            Debug.Log("Move Piece Result");

            using (FastBufferWriter writer = new FastBufferWriter(128, allocator: Unity.Collections.Allocator.Temp, 10240))
            {
                writer.WriteValueSafe(CommandType.MovePieceResult);
                writer.WriteValueSafe(succeededed);
                writer.WriteNetworkSerializable(Logic.GameData);

                SendToAll(writer);
            }
        }

        private void SendToAll(FastBufferWriter writer)
        {
            foreach (var player in Players)
            {
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("FromServer", player.ClientID, writer, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }
        #endregion
    }
}