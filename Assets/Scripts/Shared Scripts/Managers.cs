using Castling.Shared;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;
    public Camera CameraWhite;
    public Camera CameraBlack;

    public GameObject InitUI;

    public TMP_Text ShowJoinCode;
    public TMP_InputField InputJoinCode;

    private BoardEntity board;
    private Player player;

    public ServerManager Server;
    public ClientManager Client;

    public static Managers Instance => instance;

    public BoardEntity Board { get => board; private set => board = value; }
    public Player Player { get => player; private set => player = value; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Server = new ServerManager();
        Client = new ClientManager();

        Server.Init();
        Client.Init();
    }

    public void StartServer()
    {
        // NetworkManager.Singleton.StartServer();
        Debug.Log("Server로는 동작하지 않습니다. Host로 실행해주세요.");
    }

    public void StartClient()
    {
        //var data = await RelayManager.JoinRelay(InputJoinCode.text, "production");
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIdBytes, data.Key, data.ConnectionData);
        NetworkManager.Singleton.StartClient();

        //string cleanedJoinCode = RelayManager.CleanJoinCode(InputJoinCode.text);
        //if (string.IsNullOrWhiteSpace(cleanedJoinCode))
        //{
        //    Debug.LogError("Join Code is empty or invalid after cleaning.");
        //    return;
        //}
        Debug.Log("이제부터 이 컴퓨터는 Client입니다.");
    }

    public void StartHost()
    {
        //var data = await RelayManager.SetupRelay(10, "production");
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIdBytes, data.Key, data.ConnectionData);
        NetworkManager.Singleton.StartHost();
        //ShowJoinCode.text = data.JoinCode;
        Debug.Log("이제부터 이 컴퓨터는 Host입니다.");
    }


    public void GameStart(GameData gameData)
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        board = GameObject.Find("Board").GetComponent<BoardEntity>();

        if (gameData.BlackClientID == NetworkManager.Singleton.LocalClientId)
        {
            player.Init(gameData.BlackClientID, TeamColor.Black);
            CameraBlack.gameObject.SetActive(true);
            CameraWhite.gameObject.SetActive(false);
        }
        else if (gameData.WhiteClientID == NetworkManager.Singleton.LocalClientId)
        {
            player.Init(gameData.WhiteClientID, TeamColor.White);
            CameraBlack.gameObject.SetActive(false);
            CameraWhite.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Client ID Error!");
        }

        board.Init(gameData);
        InitUI.gameObject.SetActive(false);
    }
}