using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Kraken : NetworkBehaviour
{
    public float speed = 1f;
    public float fireCooldown = 1f;
    private float fireTimer = 0f;
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private FireSpot[] fireSpots;
    [SerializeField] private float bulletLifetime = 20f;

    [SerializeField] private float minSfxDelay;
    [SerializeField] private float maxSfxDelay;

    private float _currentSFXTimer;

    bool isAttacking = false;

    bool _hasStarted = false;

    private Health _health;
    private Transform closestTarget;

    private void Start()
    {
        InvokeRepeating("UpdateClosestTarget", 0f, 1f);
        _health = GetComponent<Health>();
        isAttacking = false;
    }

    // Function to update the closest target
    private void UpdateClosestTarget()
    {
        closestTarget = GetClosestTarget(transform);
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

    private void PlayKrakenSound()
    {
        // SFX HERE
    }

    private void Update()
    {
        _currentSFXTimer -= Time.deltaTime;
        if(_currentSFXTimer <= 0)
        {
            PlayKrakenSound();
            _currentSFXTimer = Random.Range(minSfxDelay, maxSfxDelay);
        }
        if(!NetworkServer.active || !_hasStarted) { return; }
        // Move towards the closest target
        if (closestTarget != null)
        {
            MoveTowardsTarget(closestTarget);
        }

        if (!isAttacking)
        {
            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0f)
            {
                // Reset the fireTimer
                fireTimer = fireCooldown;

                // Choose an attack based on health
                if (_health.CurrentHealth > (float) _health.MaxHealth / 2)
                {
                    PerformAttack((AttackType)Random.Range(0, 3));
                    
                }
                else
                {
                    PerformAttack((AttackType)Random.Range(0, 5));
                }
            }
        }
    }

    // Function to get the closest target
    public Transform GetClosestTarget(Transform krakenPosition)
    {
        float distanceToFront = Vector3.Distance(krakenPosition.position, ShipLocations.Front.position);
        float distanceToBack = Vector3.Distance(krakenPosition.position, ShipLocations.Back.position);

        if (distanceToFront < distanceToBack)
        {
            return ShipLocations.Front;
        }
        else
        {
            return ShipLocations.Back;
        }
    }

    // Function to move the kraken towards the target
    private void MoveTowardsTarget(Transform targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
    }
    
    public enum AttackType
    {
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        Attack5
    }

    public void PerformAttack(AttackType attackType)
    {
        switch (attackType)
        {
            
            case AttackType.Attack1:
                //StartCoroutine(OscillateAttack());
                break;
            case AttackType.Attack2:
                RandomAttack();
                break;
            case AttackType.Attack3:
                QuadDirectionalAttack();
                break;
            case AttackType.Attack4:
                // SpiralAttack();
                break;
            case AttackType.Attack5:
                DirectionAttack();
                break;
            default:
                Debug.LogError("Invalid attack type");
                break;
        }
    }

    public IEnumerator OscillateAttack()
    {
        Debug.Log("Oscillating attack");
        isAttacking = true;
        int[] oscillatingOrder = new int[] { 0, 7, 1, 6, 2, 5, 3, 4 }; 
        foreach (int index in oscillatingOrder)
        {
            if (index < fireSpots.Length)
            {
                var fireSpot = fireSpots[index];
                
                FireBulletFrom(fireSpot.transform.position, Vector2.up);
                yield return new WaitForSeconds(0.2f);
            }
        }
        isAttacking = false;
    }

    public void RandomAttack()
    {
        Debug.Log("Random attack");
        isAttacking = true;
        foreach (var fireSpot in fireSpots)
        {
            FireBulletFrom(fireSpot.transform.position, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        }
        isAttacking = false;
    }
    
    public void QuadDirectionalAttack()
    {
        Debug.Log("Quad directional attack");
        isAttacking = true;
        foreach (var fireSpot in fireSpots)
        {
            FireBulletFrom(fireSpot.transform.position, fireSpot.direction);
        }
        isAttacking = false;
    }

    public void DirectionAttack()
    {
        Debug.Log("Direction attack");
        isAttacking = true;
        foreach (var fireSpot in fireSpots)
        {
            Vector2 direction = (ShipLocations.CloseFront.position - fireSpot.transform.position).normalized;
            FireBulletFrom(fireSpot.transform.position, direction);
        }
        isAttacking = false;
    }

    [Server]
    private void FireBulletFrom(Vector2 pos, Vector2 direction)
    {
        SpawnBullet(pos, direction);
        RpcSpawnBullet(pos, direction);
    }

    [ClientRpc]
    private void RpcSpawnBullet(Vector2 pos, Vector2 direction)
    {
        SpawnBullet(pos, direction);
    }


    private void SpawnBullet(Vector2 pos, Vector2 dir)
    {
        var bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
        Destroy(bullet, bulletLifetime);
    }
}