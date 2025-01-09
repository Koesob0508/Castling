using Castling.Shared;
using NUnit.Framework;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class ClientManager
{
    private NetworkManager network;
    public void Init()
    {
        network = NetworkManager.Singleton;
        network.CustomMessagingManager.RegisterNamedMessageHandler("FromServer", OnReceivedServerMessage);
    }

    private void OnReceivedServerMessage(ulong clientID, FastBufferReader reader)
    {
        reader.ReadValueSafe(out ushort commandType);
        GameData gameData;

        switch (commandType)
        {
            case CommandType.TryMovePiece:
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
            player.Init(gameData.BlackClientID, eColor.Black);
            Managers.Instance.CameraBlack.gameObject.SetActive(true);
            Managers.Instance.CameraWhite.gameObject.SetActive(false);
        }
        else if (gameData.WhiteClientID == network.LocalClientId)
        {
            player.Init(gameData.WhiteClientID, eColor.White);
            Managers.Instance.CameraBlack.gameObject.SetActive(false);
            Managers.Instance.CameraWhite.gameObject.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("Client ID Error!");
        }

    }

    private void MovePieceResult(string uid, GameData gameData)
    {

    }

}