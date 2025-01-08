using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;
    public Camera CameraWhite;
    public Camera CameraBlack;

    public GameObject InitUI;

    public TMP_Text message;

    public static Managers Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartServer()
    {
        // NetworkManager.Singleton.StartServer();
        Debug.Log("Server로는 동작하지 않습니다. Host로 실행해주세요.");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("이제부터 이 컴퓨터는 Client입니다.");
        Register();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("이제부터 이 컴퓨터는 Host입니다.");
        Register();
    }

    private void Register()
    {
        // JustChat 이벤트에 OnReceived를 등록한다.
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("JustChat", OnReceived);
    }

    private void OnReceived(ulong clientID, FastBufferReader reader)
    {
        // 메시지를 받으면 string reply로 읽고, reply를 로그로 남긴다.
        reader.ReadValueSafe(out string reply);
        Debug.Log(reply);
    }

    public void Send()
    {
        Debug.Log("Send");
        // 아직 받는 사람 정보가 없다.
        ulong targetClientID = 0;

        FastBufferWriter writer = new FastBufferWriter(size: 128, allocator: Allocator.Temp);
        // message 내용을 싣는다.
        writer.WriteValueSafe(message.text);
        NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced;

        if (NetworkManager.Singleton.IsHost)
        {
            // ConnectedClientsIds에 접근할 수 있는 것은 Host니까 가능함
            // Client에서는 안된다.
            List<ulong> clientIDs = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
            clientIDs.Remove(NetworkManager.Singleton.LocalClientId);

            // 메시지를 보낼 때, List 값을 주면 List에 있는 모든 Client에게 메시를 보낸다.
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("JustChat", clientIDs, writer, networkDelivery);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Client에서는 ServerClientId는 알 수 있다.
            targetClientID = NetworkManager.ServerClientId;

            // Server에게 메시지 보내기
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("JustChat", targetClientID, writer, networkDelivery);
        }
        else
        {
            return;
        }
    }
}