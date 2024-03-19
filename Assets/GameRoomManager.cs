using System;
using UnityEngine;

public class GameRoomManager : MonoBehaviour
{
    public static GameRoomManager Instance { get; private set; }

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

    public void StartNewGame(GameRoom room)
    {
        foreach (Player player in room.currentPlayers)
        {
            NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.startNewGame.ToString(), player.connection, TransportPipeline.ReliableAndInOrder);
        }
    }

    public void SetUp(GameRoom room)
    {
        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.opponentUsername + "," + room.currentPlayers[0].username, room.currentPlayers[1].connection, TransportPipeline.ReliableAndInOrder);
        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.opponentUsername + "," + room.currentPlayers[1].username, room.currentPlayers[0].connection, TransportPipeline.ReliableAndInOrder);
    }

    public void LeaveRoom(int connectionID)
    {
        foreach (GameRoom gameRoom in LobbyManager.Instance.activeGameRooms)
        {
            for (int i = 0; i < gameRoom.currentPlayers.Count; i++)
            {
                if (gameRoom.currentPlayers[i].connection == connectionID)
                {
                    gameRoom.currentPlayers.RemoveAt(i);

                    if (gameRoom.currentPlayers.Count <= 0)
                    {
                        LobbyManager.Instance.activeGameRooms.RemoveAt(i);
                    }
                    NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.leaveGameRoom.ToString(),
                        connectionID, TransportPipeline.ReliableAndInOrder);
                    return;
                }
            }
        }
    }

    public void OnPlayerDisconnected(int connectionID)
    {
        foreach (GameRoom gameRoom in LobbyManager.Instance.activeGameRooms)
        {
            for (int i = 0; i < gameRoom.currentPlayers.Count; i++)
            {
                if (gameRoom.currentPlayers[i].connection == connectionID)
                {
                    gameRoom.currentPlayers.RemoveAt(i);

                    if (gameRoom.currentPlayers.Count <= 0)
                    {
                        LobbyManager.Instance.activeGameRooms.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < LobbyManager.Instance.activePlayers.Count; i++)
        {
            if (LobbyManager.Instance.activePlayers[i].connection == connectionID)
            {
                LobbyManager.Instance.activePlayers.RemoveAt(i);
            }
        }
    }
}
