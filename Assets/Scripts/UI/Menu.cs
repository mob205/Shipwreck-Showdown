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


    // Start is called before the first frame update
    void Start()
    {

        manager = NetworkManager.singleton;
    }

    public void JoinServer()
    {
        if(!string.IsNullOrEmpty(EnterIP.text))
        {
            manager.networkAddress = EnterIP.text.Remove(EnterIP.text.Length - 1); // no idea why this is the only way it works
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
