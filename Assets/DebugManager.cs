using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public Button addPlayer;
    public Button forceStartGame;

    private void Start()
    {
        addPlayer.onClick.AddListener(Debug_AddPlayerToGameRoom);
        forceStartGame.onClick.AddListener(Debug_ForceStartGame);
    }

    public void Debug_AddPlayerToGameRoom()
    {
        LobbyManager.Instance.activeGameRooms[0].currentPlayers.Add(new Player("DebugUser", 2));
    }

    public void Debug_ForceStartGame()
    {
        GameRoomManager.Instance.StartNewGame(LobbyManager.Instance.activeGameRooms[0]);
    }
}
