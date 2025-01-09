using Castling.Server;
using Castling.Shared;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager
{
    private Queue<ulong> ReadyPlayers;
    private Dictionary<ulong, PlayerInfo> PlayingPlayers;
    private Dictionary<int, IGameSession> Sessions;

    public void Init()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;

        ReadyPlayers = new();
        PlayingPlayers = new();
        Sessions = new();
    }

    public void Clear()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnConnect;
    }

    private void OnConnect(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            OnMyClientJoin();
        }
        else
        {
            OnOtherClientJoin(clientID);
        }
    }

    private void OnMyClientJoin()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Clear();
            return;
        }

        ReadyPlayers.Enqueue(NetworkManager.Singleton.LocalClientId);
        WhenHasTwoConnectedClients();
    }

    private void OnOtherClientJoin(ulong clientID)
    {
        if (ReadyPlayers.Contains(clientID)) return;

        ReadyPlayers.Enqueue(clientID);
        WhenHasTwoConnectedClients();
    }

    private void WhenHasTwoConnectedClients()
    {
        if (ReadyPlayers.Count >= 2)
        {
            var player1 = ReadyPlayers.Dequeue();
            var player2 = ReadyPlayers.Dequeue();

            var player1Info = new PlayerInfo() { ClientID = player1 };
            var player2Info = new PlayerInfo() { ClientID = player2 };

            var playerInfos = new List<PlayerInfo> { player1Info, player2Info };

            OpenSession(playerInfos);

            PlayingPlayers[player1] = player1Info;
            PlayingPlayers[player2] = player2Info;
        }
        else
        {
            Debug.Log($"Current connected clients: {ReadyPlayers.Count}");
        }
    }

    private void OpenSession(List<PlayerInfo> playerInfos)
    {
        var sessionID = Sessions.Count;
        var gameSession = new DefaultGameSession();
        gameSession.Init(sessionID, playerInfos);
        Sessions[sessionID] = gameSession;
    }

    private void CloseSession(int sessionID)
    {
        if (Sessions.TryGetValue(sessionID, out var session))
        {
            session.Clear();
            Sessions.Remove(sessionID);
        }

    }
}

public class PlayerInfo
{
    public ulong ClientID;
}