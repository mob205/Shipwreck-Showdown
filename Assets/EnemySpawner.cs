using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _enemy;
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;

    [SerializeField] private Transform[] _spawns;
    [SerializeField] private AudioEvent splash;

    private float _currentTimer;
    private bool _hasStarted = false;
    private AudioSource _source;

    public override void OnStartClient()
    {
        if(!netIdentity.isServer)
        {
            Destroy(gameObject);
            return;
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        ControlInteractor.OnPossessed.AddListener(OnPossessed);
    }
    private void OnPossessed(ControlInteractor interactor)
    {
        _hasStarted = true;
    }

    private void Update()
    {
        if(netIdentity.isServer && _hasStarted)
        {
            _currentTimer -= Time.deltaTime;
            if (_currentTimer <= 0)
            {
                _source = GetComponent<AudioSource>();
                SpawnEnemy();
                _currentTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
            }
        }
    }
    [Server]
    private void SpawnEnemy()
    {
        var randomSpawn = _spawns[Random.Range(0, _spawns.Length - 1)];
        var enemy = Instantiate(_enemy, randomSpawn.position, randomSpawn.rotation);
        RpcOnEnemySpawn();
        NetworkServer.Spawn(enemy);
    }

    [ClientRpc]
    private void RpcOnEnemySpawn()
    {
        splash.Play(_source);
    }
}
