using Castling.Server;
using Castling.Shared;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager
{
    private Queue<ulong> ReadyPlayers;
    private Dictionary<ulong, PlayerInfo> PlayingPlayers;
    private List<IGameSession> Sessions;

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
        if(clientID == NetworkManager.Singleton.LocalClientId)
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
        if (NetworkManager.Singleton.IsHost)
        {
            ReadyPlayers.Enqueue(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.Log("This is not host client");

            Clear();
            return;
        }
    }

    private void OnOtherClientJoin(ulong clientID)
    {
        if (ReadyPlayers.Contains(clientID)) return;

        ReadyPlayers.Enqueue(clientID);

        if(ReadyPlayers.Count > 2)
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
    }

    private void OpenSession(List<PlayerInfo> playerInfos)
    {
        var gameSession = new DefaultGameSession();
        Sessions.Add(gameSession);
        gameSession.Init(playerInfos);

    }

    
}

public class PlayerInfo
{
    public ulong ClientID;
}