using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string username;
    public int connection;

    public Player(string userName, int connectionID)
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

    public void CreateGameRoom(string receivedMessage, int connectionID)
    {
        string[] gameRoomName;
        gameRoomName = receivedMessage.Split(",");

        foreach (Player player in activePlayers)
        {
            if (player.connection == connectionID)
            {
                if (!CheckForExistingRoom(gameRoomName[1]))
                {
                    GameRoom gameRoom = new GameRoom(gameRoomName[1]);
                    gameRoom.currentPlayers.Add(player);
                    activeGameRooms.Add(gameRoom);
                    NetworkServerProcessing.SendMessageToClient(ClientToServerSignifiers.createGameRoom.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
                }
                else
                {
                    NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.failedToCreateRoom.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
                }
            }
        }
    }

    public void JoinExistingGameRoom(string receivedMessage,  int connectionID)
    {
        string[] gameRoomName;
        gameRoomName = receivedMessage.Split(",");
        
        foreach (Player player in activePlayers)
        {
            if (player.connection == connectionID)
            {
                if (CheckForExistingRoom(gameRoomName[1]))
                {
                    foreach (GameRoom gameRoom in activeGameRooms)
                    {
                        if (gameRoomName[1] == gameRoom.roomName)
                        {
                            if (gameRoom.currentPlayers.Count < 2)
                            {
                                gameRoom.currentPlayers.Add(player);
                            }
                            else
                            {
                                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.gameRoomFull.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
                            }
                        }
                    }
                }
            }
        }
    }

    public void LogOut(int connectionID)
    {
        for (int i = 0; i < activePlayers.Count; i++)
        {
            if (activePlayers[i].connection == connectionID)
            {
                activePlayers.RemoveAt(i);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.logOut.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
            }
        }
    }
}