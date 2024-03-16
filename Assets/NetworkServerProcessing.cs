using UnityEngine;

public static class NetworkServerProcessing
{
    #region Send and Receive Data Functions
    public static void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.RegisterAccount)
        {
            AccountsManager.Instance.RegisterNewAccountCredentials(msg, clientConnectionID);
        }
        else if (signifier == ClientToServerSignifiers.LoginAccount)
        {
            AccountsManager.Instance.CheckLoginCredentials(msg, clientConnectionID);
        }
        else if (signifier == ClientToServerSignifiers.createGameRoom)
        {
            LobbyManager.Instance.CreateGameRoom(msg, clientConnectionID);
        }
        else if (signifier == ClientToServerSignifiers.joinExistingRoom)
        {
            LobbyManager.Instance.CreateGameRoom(msg, clientConnectionID);
        }
        else if (signifier == ServerToClientSignifiers.logOut)
        {
            LobbyManager.Instance.LogOut(clientConnectionID);
        }

        //gameLogic.DoSomething();
    }
    public static void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }

    #endregion

    #region Connection Events

    public static void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client connection, ID == " + clientConnectionID);
    }
    public static void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client disconnection, ID == " + clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkServer networkServer;
   // static GameLogic gameLogic;

    public static void SetNetworkServer(NetworkServer NetworkServer)
    {
        networkServer = NetworkServer;
    }
    public static NetworkServer GetNetworkServer()
    {
        return networkServer;
    }
    //public static void SetGameLogic(GameLogic GameLogic)
    //{
    //    gameLogic = GameLogic;
    //}

    #endregion
}

#region Protocol Signifiers
public static class ClientToServerSignifiers
{
    public const int RegisterAccount = 0;
    public const int LoginAccount = 1;
    public const int createGameRoom = 3;
    public const int joinExistingRoom = 5;
}

public static class ServerToClientSignifiers
{
    public const int successfulLogin = 2;
    public const int failedLogin = 5;
    public const int failedToCreateRoom = 6;
    public const int usernameTaken = 7;
    public const int gameRoomFull = 8;
    public const int leaveGameRoom = 1;
    public const int logOut = 4;
}

#endregion