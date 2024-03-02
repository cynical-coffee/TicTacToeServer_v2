using System.IO;
using Unity.Networking.Transport;
using UnityEngine;

public static class Signifiers
{
    public const string RegisterAccountSignifier = "0";
    public const string LoginAccountSignifier = "1";
    public const string successfulLoginSignifier = "2";
}

public class AccountsManager : MonoBehaviour
{
    public static AccountsManager Instance { get; private set; }

    private const string accountDataSignifier = "0";
    //private List<string> LoggedinAccounts = new List<string>();
   //private GameObject verticalLayout;
    
    [SerializeField] private GameObject accountNameText;

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
        //verticalLayout = GameObject.Find("VerticalLayout");
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
            NetworkServer.Instance.SendMessageToClient("Username is taken.", connectionID);
        }
    }

    public void CheckLoginCredentials(string receivedMessage, NetworkConnection connectionID)
    {
        bool accountExists = false;
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
                        accountExists = true;
                        break;
                    }
                }
            }
        }

        if (accountExists)
        {
            NetworkServer.Instance.SendMessageToClient($"Welcome Back, {existingAccountCredentials[1]}!", connectionID);
            NetworkServer.Instance.SendMessageToClient(Signifiers.successfulLoginSignifier, connectionID);
        }
        else
        {
            NetworkServer.Instance.SendMessageToClient("Username/Password is incorrect or does not exist.", connectionID);
        }
    }

    //private void DisplayLoggedInAccounts()
    //{
    //    if (LoggedinAccounts != null)
    //    {
    //        foreach (string account in LoggedinAccounts)
    //        {
    //            GameObject userNameText = Instantiate(accountNameText, verticalLayout.transform);
    //            userNameText.GetComponent<TMP_Text>().text = account;
    //        }
    //    }
    //}
}
