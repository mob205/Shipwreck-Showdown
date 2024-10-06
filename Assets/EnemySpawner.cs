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

    private float _currentTimer;

    public override void OnStartClient()
    {
        if(!netIdentity.isServer)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Update()
    {
        if(netIdentity.isServer)
        {
            _currentTimer -= Time.deltaTime;
            if (_currentTimer <= 0)
            {
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
        NetworkServer.Spawn(enemy);
    }
}
