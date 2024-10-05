using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EndStateController : NetworkBehaviour
{
    [SerializeField] private GameObject _gameoverUI;
    private int _numPlayersAlive;

    public static EndStateController Instance;


    private void Awake()
    {
        Instance = this;
        ShipwreckNetworkManager.OnPlayerConnect += OnServerAddPlayer;
        ShipwreckNetworkManager.OnPlayerDisconnect += OnServerRemovePlayer;
    }


    public override void OnStartServer()
    {
        base.OnStartServer();
        FindObjectOfType<ShipMovement>().GetComponent<Health>().OnDeath.AddListener(OnShipDeath);
    }

    private void OnServerAddPlayer(NetworkIdentity identity)
    {
        ++_numPlayersAlive;
        identity.GetComponent<Health>().OnDeath.AddListener(OnPlayerDeath);
    }
    private void OnServerRemovePlayer(NetworkIdentity identity)
    {
        identity.GetComponent<PlayerController>().Unpossess();
        --_numPlayersAlive;
    }

    private void OnPlayerDeath(Health health)
    {
        --_numPlayersAlive;
        if(_numPlayersAlive <= 0)
        {
            RpcSpawnUI();
        }
    }

    private void OnShipDeath(Health health)
    {
        var players = FindObjectsOfType<Health>();
        foreach(var player in players)
        {
            player.ModifyHealth(-999, gameObject);
        }
    }

    [ClientRpc]
    private void RpcSpawnUI()
    {
        _gameoverUI.SetActive(true);
    }


    public void Restart()
    {
        CmdReset();
    }

    [Command(requiresAuthority = false)]
    public void CmdReset()
    {
        ShipwreckNetworkManager.Instance.ReloadScene();
    }

}
