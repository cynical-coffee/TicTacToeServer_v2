using System.IO;
using UnityEngine;

public class AccountsManager : MonoBehaviour
{
    public static AccountsManager Instance { get; private set; }

    private const string accountDataSignifier = "0";

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

    private bool CheckForExistingAccount(string username)
    {
        if (!File.Exists("AccountsData.txt"))
        {
            return false;
        }

        string currentLine;

        using (StreamReader sReader = new StreamReader("AccountsData.txt"))
        {
            string[] currentAccount;

            while ((currentLine = sReader.ReadLine()) != null)
            {
                currentAccount = currentLine.Split(",");

                if (username == currentAccount[1])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void RegisterNewAccountCredentials(string receivedMessage, int connectionID)
    {
        string[] newAccountCredentials;
        newAccountCredentials = receivedMessage.Split(",");

        if (!CheckForExistingAccount(newAccountCredentials[1]))
        {
            using (StreamWriter sWriter = new StreamWriter("AccountsData.txt", true))
            {
                sWriter.WriteLine(accountDataSignifier + "," + newAccountCredentials[1] + "," + newAccountCredentials[2]);
            }
        }
        else
        {
            NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.usernameTaken.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
        }
    }

    public void CheckLoginCredentials(string receivedMessage, int connectionID)
    {
        string[] existingAccountCredentials;
        existingAccountCredentials = receivedMessage.Split(",");

        if (CheckForExistingAccount(existingAccountCredentials[1]))
        {
            using (StreamReader sReader = new StreamReader("AccountsData.txt"))
            {
                string currentLine = "";

                while ((currentLine = sReader.ReadLine()) != null)
                {
                    string[] currentAccount = currentLine.Split(",");

                    if (existingAccountCredentials[2] == currentAccount[2])
                    {
                        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.successfulLogin.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
                        Player player = new Player(existingAccountCredentials[1], connectionID);
                        LobbyManager.Instance.activePlayers.Add(player);
                        return;
                    }
                }
            }
        }
        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.failedLogin.ToString(), connectionID, TransportPipeline.ReliableAndInOrder);
    }
}
