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
        Instance = (ShipwreckNetworkManager) singleton;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        conn.identity.gameObject.GetComponent<PlayerController>().AssignColor(_color++);
        OnPlayerConnect?.Invoke(conn.identity);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        OnPlayerDisconnect?.Invoke(conn.identity);
        base.OnServerDisconnect(conn);
    }
    public override void Update()
    {
        base.Update();
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
