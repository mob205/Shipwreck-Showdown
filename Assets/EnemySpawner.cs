using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Goon _enemy;
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;

    private void Start()
    {

    }
}
