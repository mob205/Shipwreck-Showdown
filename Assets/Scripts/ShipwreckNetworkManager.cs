using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ShipwreckNetworkManager : NetworkManager
{
    public static event Action<NetworkIdentity> OnPlayerConnect;
    public static event Action<NetworkIdentity> OnPlayerDisconnect;

    public static ShipwreckNetworkManager Instance;
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
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        OnPlayerDisconnect?.Invoke(conn.identity);
        base.OnServerDisconnect(conn);
    }
}
