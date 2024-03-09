using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkServerProcessing : MonoBehaviour
{
    public static NetworkServerProcessing Instance { get; private set; }

    private NetworkServer networkServer;

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

        networkServer = FindObjectOfType<NetworkServer>();
    }

    public void ProcessMessageFromClient(string msg, NetworkConnection networkConnection)
    {
        Debug.Log("Msg received = " + msg + "," + networkConnection.InternalId);

        if (msg.StartsWith(Signifiers.RegisterAccountSignifier))
        {
            AccountsManager.Instance.RegisterNewAccountCredentials(msg, networkConnection);
        }

        if (msg.StartsWith(Signifiers.LoginAccountSignifier))
        {
            AccountsManager.Instance.CheckLoginCredentials(msg, networkConnection);
        }

        if (msg.StartsWith(Signifiers.createGameRoomSignifier))
        {
            foreach (Player player in LobbyManager.Instance.activePlayers)
            {
                if (player.connection == networkConnection)
                {
                    LobbyManager.Instance.CreateGameRoom(msg, player, networkConnection);
                }
            }
        }

        if (msg.StartsWith(Signifiers.logOutSignifier))
        {
            LobbyManager.Instance.LogOut(networkConnection);
        }
    }
    public void SendMessageToClient(string msg, NetworkConnection networkConnection)
    {
        networkServer.SendMessageToClient(msg, networkConnection);
    }
}

public static class Signifiers
{
    public const string RegisterAccountSignifier = "0";
    public const string LoginAccountSignifier = "1";
    public const string successfulLoginSignifier = "2";
    public const string createGameRoomSignifier = "3";
    public const string logOutSignifier = "4";
}
