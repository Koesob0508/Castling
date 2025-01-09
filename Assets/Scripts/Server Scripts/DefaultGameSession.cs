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

            Logic.OnGameStarted += SendGameStarted;
            Logic.OnGameEnded += SendGameEnded;
            Logic.OnTurnChanged += SendTurnChanged;
            Logic.OnMoveSucceeded += () => SendMovePieceResult(true);
            Logic.OnMoveFailed += () => SendMovePieceResult(false);

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("FromClient", OnReceivedClientMessage);
            
            Logic.Init();
        }

        public void Clear()
        {
            Logic.Clear();
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
            Logic.TryMovePiece(pieceUID, destinationX, destinationY);
        }

        #region Send

        private void SendGameStarted()
        {
            Debug.Log("Start Game");

            using (FastBufferWriter writer = new FastBufferWriter(128, allocator: Unity.Collections.Allocator.Temp, 10240))
            {
                writer.WriteValueSafe(CommandType.StartGame);
                writer.WriteNetworkSerializable(Logic.GameData);

                SendToAll(writer);
            }
        }

        private void SendGameEnded(ulong winnerID)
        {
            using (FastBufferWriter writer = new FastBufferWriter(128, allocator: Unity.Collections.Allocator.Temp, 10240))
            {
                writer.WriteValueSafe(CommandType.EndGame);
                writer.WriteValueSafe(winnerID);

                SendToAll(writer);
            }
        }

        private void SendTurnChanged()
        {
            Debug.Log("Start Game");

            using (FastBufferWriter writer = new FastBufferWriter(128, allocator: Unity.Collections.Allocator.Temp, 10240))
            {
                writer.WriteValueSafe(CommandType.ChangeTurn);
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