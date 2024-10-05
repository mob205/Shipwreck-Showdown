using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class Menu : MonoBehaviour
{
    private string input;
    [SerializeField] private Button JoinButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private TextMeshProUGUI EnterIP;

    private NetworkManager manager;
    private bool joinAttempt;
    private TextMeshProUGUI JoinText;


    // Start is called before the first frame update
    void Start()
    {

        manager = NetworkManager.singleton;

        JoinText = JoinButton.GetComponentInChildren<TextMeshProUGUI>();
        
    }

    private void InitializeServer()
    {

        manager.StartServer();
        Debug.Log("giggling");

    }

    public void JoinServer()
    {

        if(joinAttempt)
        {
            manager.StopClient();
            joinAttempt = false;
            JoinText.text = "JOIN";
            return;

        }

        if(!string.IsNullOrEmpty(EnterIP.text))
        {
            manager.networkAddress = EnterIP.text;
            joinAttempt = true;
            manager.StartClient();

        }

    }

    public void Host()
    {

        manager.StartHost();
        HostButton.interactable = false;

    }
    public void ReadIPField(string ip) 
    {

        input = ip;
        Debug.Log(input);


    }
}
