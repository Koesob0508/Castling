using Castling.Shared;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor.VersionControl;
using UnityEngine;

public class ClientManager
{
    private NetworkManager network;

    public void Init()
    {
        network = NetworkManager.Singleton;
        network.OnClientConnectedCallback += OnConnect;
    }

    private void OnConnect(ulong clientID)
    {
        if(clientID == network.LocalClientId)
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
        network.CustomMessagingManager.RegisterNamedMessageHandler("FromServer", OnReceivedServerMessage);
    }

    private void OnOtherClientJoin(ulong clientID)
    {
        if(!network.IsHost)
            Debug.Log("뭔가 잘못됨");
    }

    private void OnReceivedServerMessage(ulong clientID, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ushort commandType);
        GameData gameData;

        switch (commandType)
        {
            case CommandType.TryMovePiece:
                // Client에서는 실행하지 않음.
                break;
            case CommandType.StartGame:
                reader.ReadValueSafe(out gameData);
                StartGame(gameData);
                // Server에서는 실행하지 않음
                break;
            case CommandType.EndGame:
                // Server에서는 실행하지 않음
                break;
            case CommandType.MovePieceResult:
                reader.ReadValueSafe(out bool result);
                if (result)
                {
                    reader.ReadValueSafe(out string uid);
                    reader.ReadValueSafe(out gameData);
                    MovePieceResult(uid, gameData);
                }
                break;
        }
    }
    private void StartGame(GameData gameData)
    {
        Player player = GameObject.Find("Player").GetComponent<Player>();
        if (gameData.BlackClientID == network.LocalClientId)
        {
            player.Init(gameData.BlackClientID, TeamColor.Black);
            Managers.Instance.CameraBlack.gameObject.SetActive(true);
            Managers.Instance.CameraWhite.gameObject.SetActive(false);
        }
        else if (gameData.WhiteClientID == network.LocalClientId)
        {
            player.Init(gameData.WhiteClientID, TeamColor.White);
            Managers.Instance.CameraBlack.gameObject.SetActive(false);
            Managers.Instance.CameraWhite.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Client ID Error!");
        }

        Managers.Instance.Board.Init(gameData);

    }

    private void MovePieceResult(string uid, GameData gameData)
    {

    }

    public void SendTryMovePiece(string pieceUID, Vector2Int destination)
    {
        Debug.Log("SendTryMovePiece");
        // 아직 받는 사람 정보가 없다.
        ulong targetClientID = 0;

        FastBufferWriter writer = new FastBufferWriter(size: 128, allocator: Allocator.Temp);
        // message 내용을 싣는다.
        writer.WriteValueSafe(CommandType.TryMovePiece);
        writer.WriteValueSafe(pieceUID);
        writer.WriteValueSafe(destination.x);
        writer.WriteValueSafe(destination.y);

        NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced;

        if (NetworkManager.Singleton.IsHost)
        {
            // ConnectedClientsIds에 접근할 수 있는 것은 Host니까 가능함
            // Client에서는 안된다.
            List<ulong> clientIDs = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
            clientIDs.Remove(NetworkManager.Singleton.LocalClientId);

            // 메시지를 보낼 때, List 값을 주면 List에 있는 모든 Client에게 메시를 보낸다.
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("FromClient", clientIDs, writer, networkDelivery);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Client에서는 ServerClientId는 알 수 있다.
            targetClientID = NetworkManager.ServerClientId;

            // Server에게 메시지 보내기
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("FromClient", targetClientID, writer, networkDelivery);
        }
        else
        {
            return;
        }
    }
}