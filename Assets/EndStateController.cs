using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EndStateController : NetworkBehaviour
{
    [SerializeField] private GameObject _gameoverUI;
    [SerializeField] private GameObject _victoryUI;
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

        FindObjectOfType<Kraken>().GetComponent<Health>().OnDeath.AddListener(OnKrakenDeath);
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
            RpcSpawnEndUI();
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

    private void OnKrakenDeath(Health health)
    {
        var players = FindObjectsOfType<Health>();
        foreach(var player in players)
        {
            player.IsVulnerable = false;
        }
        NetworkServer.Destroy(health.gameObject);
        RpcSpawnVictoryUI();
    }

    [ClientRpc]
    private void RpcSpawnEndUI()
    {
        _gameoverUI.SetActive(true);
    }

    [ClientRpc]
    private void RpcSpawnVictoryUI()
    {
        _victoryUI.SetActive(true);
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
