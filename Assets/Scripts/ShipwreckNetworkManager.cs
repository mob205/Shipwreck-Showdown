using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ShipwreckNetworkManager : NetworkManager
{
    public static event Action<NetworkIdentity> OnPlayerConnect;
    public static event Action<NetworkIdentity> OnPlayerDisconnect;
    public static event Action OnServerStarted;

    public static ShipwreckNetworkManager Instance;

    private int _color = 0;
    public override void Awake()
    {
        base.Awake();
        if(Instance)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        OnPlayerConnect?.Invoke(conn.identity);
        conn.identity.gameObject.GetComponent<PlayerController>().AssignColor(_color++);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        OnPlayerDisconnect?.Invoke(conn.identity);
        base.OnServerDisconnect(conn);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        OnServerStarted?.Invoke();
    }

    [Server]
    public void ReloadScene()
    {
        _color = 0;
        OnPlayerConnect = null;
        OnPlayerDisconnect = null;

        ServerChangeScene(onlineScene);
    }
}
