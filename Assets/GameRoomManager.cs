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

    public void LeaveRoom(int connectionID)
    {
        foreach (GameRoom gameRoom in LobbyManager.Instance.activeGameRooms)
        {
            for (int i = 0; i < gameRoom.currentPlayers.Count; i++)
            {
                if (gameRoom.currentPlayers[i].connection == connectionID)
                {
                    gameRoom.currentPlayers.RemoveAt(i);
                    NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.leaveGameRoom.ToString(),
                        connectionID, TransportPipeline.ReliableAndInOrder);
                }
            }
        }
    }
}
