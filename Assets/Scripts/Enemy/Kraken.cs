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
    [SerializeField] private float damageFlashDuration;

    [SerializeField] private float minSfxDelay;
    [SerializeField] private float maxSfxDelay;
    [SerializeField] private AudioEvent roar;
    [SerializeField] private AudioEvent death;

    private float _currentSFXTimer;

    bool isAttacking = false;

    bool _hasStarted = false;

    private Health _health;
    private Transform closestTarget;
    private AudioSource _source;
    private SpriteRenderer[] _spriteRenderers;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _health = GetComponent<Health>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
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
        _health.OnDeath.AddListener(OnDeath);
        InvokeRepeating("UpdateClosestTarget", 0f, 1f);
    }

    private void OnPossessed(ControlInteractor interactor)
    {
        _hasStarted = true;
    }

    private void OnDeath(Health health)
    {
        RpcOnDeath(transform.position);
    }

    [ClientRpc]
    private void RpcOnDeath(Vector2 position)
    {
        if(death)
        {
            death.PlayOneShot(position);
        }
    }

    public void OnDamage(Health health, int amount, GameObject attacker)
    {   
        RpcOnDamage();
    }


    [ClientRpc]
    private void RpcOnDamage()
    {
        foreach(var sprite in _spriteRenderers)
        {
            sprite.color = Color.red;
        }
        Invoke(nameof(ResetSpriteColor), damageFlashDuration);
    }
    private void ResetSpriteColor()
    {
        foreach(var sprite in _spriteRenderers)
        {
            sprite.color = Color.white;
        }
    }

    private void PlayKrakenSound()
    {
        roar.Play(_source);
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
                break;
            case AttackType.Attack2:
                RandomAttack();
                break;
            case AttackType.Attack3:
                QuadDirectionalAttack();
                break;
            case AttackType.Attack4:
                StartCoroutine(SpiralAttack());
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
    
    public IEnumerator SpiralAttack()
    {
        Debug.Log("Spiral attack");
        isAttacking = true;
        // Shoot bullets from kraken head in a full circle
        for (int i = 0; i < 20; i++)
        {
            float angle = i * 2 * Mathf.PI / 20;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            FireBulletFrom(transform.position, direction);
            // Wait a bit before shooting the next bullet
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Spiral attack done.");
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
        if(!NetworkServer.activeHost)
        {
            SpawnBullet(pos, direction);
        }
    }

    private void SpawnBullet(Vector2 pos, Vector2 dir)
    {
        var bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
        Destroy(bullet.gameObject, bulletLifetime);
    }
}