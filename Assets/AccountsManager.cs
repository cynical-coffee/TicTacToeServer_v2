using System.IO;
using Unity.Networking.Transport;
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

    public void RegisterNewAccountCredentials(string receivedMessage, NetworkConnection connectionID)
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
            NetworkServerProcessing.Instance.SendMessageToClient("Username is taken.", connectionID);
        }
    }

    public void CheckLoginCredentials(string receivedMessage, NetworkConnection connectionID)
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
                        NetworkServerProcessing.Instance.SendMessageToClient($"Welcome Back, {existingAccountCredentials[1]}!", connectionID);
                        NetworkServerProcessing.Instance.SendMessageToClient(Signifiers.successfulLoginSignifier, connectionID);
                        Player player = new Player(existingAccountCredentials[1], connectionID);
                        LobbyManager.Instance.activePlayers.Add(player);
                        return;
                    }
                }
            }
        }
        NetworkServerProcessing.Instance.SendMessageToClient("Username/Password is incorrect or does not exist.", connectionID);
    }
}
