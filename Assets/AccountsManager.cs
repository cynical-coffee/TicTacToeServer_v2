using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class AccountsManager : MonoBehaviour
{
    public static AccountsManager Instance { get; private set; }

    private const string accountDataSignifier = "0";
    private List<string> LoggedinAccounts = new List<string>();
    private GameObject verticalLayout;

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
        verticalLayout = GameObject.Find("VerticalLayout");
    }

    public void CreateNewAccountCredentials(string receivedMessage)
    {
        string[] newAccountCredentials;
        newAccountCredentials = receivedMessage.Split(",");

        using (StreamWriter sWriter = new StreamWriter("AccountsData.txt", true))
        {
            sWriter.WriteLine(accountDataSignifier + "," + newAccountCredentials[1] + "," + newAccountCredentials[2]);
        }
    }

    public void CheckLoginCredentials(string receivedMessage)
    {
        string[] clientMessage;
        string currentLine = "";
        clientMessage = receivedMessage.Split(",");

        using (StreamReader sReader = new StreamReader("AccountsData.txt"))
        {
            while ((currentLine = sReader.ReadLine()) != null)
            {
                string[] loginCredentials;
                loginCredentials = currentLine.Split(",");

                Debug.Log("Test 1");

               if (loginCredentials[1] == clientMessage[1] && loginCredentials[2] == clientMessage[2])
               {
                   Debug.Log("Test 2");

                    LoggedinAccounts.Add(loginCredentials[1]);
                    foreach (var account in LoggedinAccounts)
                    {
                        Debug.Log($"{account} is Logged in.");
                    }
                    DisplayLoggedInAccounts();
                    break;
               }
            }
        }
    }

    private void DisplayLoggedInAccounts()
    {
        if (LoggedinAccounts != null)
        {
            foreach (string account in LoggedinAccounts)
            {
                GameObject userNameText = Instantiate(accountNameText, verticalLayout.transform);
                userNameText.GetComponent<TMP_Text>().text = account;
            }
        }
    }
}
