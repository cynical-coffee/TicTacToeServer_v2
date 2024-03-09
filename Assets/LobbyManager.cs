using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Player
{
    public string username;
    public NetworkConnection connection;

    public Player(string userName, NetworkConnection connectionID)
    {
        username = userName;
        connection = connectionID;
    }
}

public class GameRoom
{
    public string roomName;
    public List<Player> currentPlayers;

    public GameRoom(string gameRoomName)
    {
        roomName = gameRoomName;
        currentPlayers = new List<Player>();
    }
}

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public List<Player> activePlayers;
    public List<GameRoom> activeGameRooms;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        activePlayers = new List<Player>();
        activeGameRooms = new List<GameRoom>();
    }

    private bool CheckForExistingRoom(string roomName)
    {
        foreach (GameRoom gameRoom in activeGameRooms)
        {
            if (gameRoom.roomName == roomName)
            {
                return true;
            }
        }
        return false;
    }

    public void CreateGameRoom(string receivedMessage, Player player, NetworkConnection connectionID)
    {
        string[] gameRoomName;
        gameRoomName = receivedMessage.Split(",");

        if (!CheckForExistingRoom(gameRoomName[1]))
        {
            GameRoom gameRoom = new GameRoom(gameRoomName[1]);
            gameRoom.currentPlayers.Add(player);
            activeGameRooms.Add(gameRoom);
            NetworkServerProcessing.Instance.SendMessageToClient(Signifiers.createGameRoomSignifier, connectionID);
        }
        else
        {
            NetworkServerProcessing.Instance.SendMessageToClient("Room Already Exists.", connectionID);
        }
    }

    public void LogOut(NetworkConnection connectionID)
    {
        for (int i = 0; i < activePlayers.Count; i++)
        {
            if (activePlayers[i].connection == connectionID)
            {
                activePlayers.RemoveAt(i);
                NetworkServerProcessing.Instance.SendMessageToClient(Signifiers.logOutSignifier + "Logged Out!", connectionID);
            }
        }
    }
}

