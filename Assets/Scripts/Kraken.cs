using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kraken : MonoBehaviour
{
    public Transform FrontTarget;
    public Transform BackTarget;
    public Transform ShipTarget;
    public Transform SeconfFrontTarget; // because why the FUCK NOT
    public float speed = 1f;
    public float fireCooldown = 1f;
    private float fireTimer = 0f;
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Tentacle[] fireSpots;
    bool isAttacking = false;
    [SerializeField] private int maxhealth;
    [SerializeField] private int currenthealth;
    private Transform closestTarget;

    private void Start()
    {
        InvokeRepeating("UpdateClosestTarget", 0f, 1f);
        currenthealth = maxhealth;
        isAttacking = false;
    }

    // Function to update the closest target
    private void UpdateClosestTarget()
    {
        closestTarget = GetClosestTarget(transform);
    }
    
    private void Update()
    {
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
                if (currenthealth > maxhealth / 2)
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
        float distanceToFront = Vector3.Distance(krakenPosition.position, FrontTarget.position);
        float distanceToBack = Vector3.Distance(krakenPosition.position, BackTarget.position);

        if (distanceToFront < distanceToBack)
        {
            return FrontTarget;
        }
        else
        {
            return BackTarget;
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
                
                FireBulletFrom(fireSpot.transform, Vector2.up);
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
            FireBulletFrom(fireSpot.transform, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        }
        isAttacking = false;
    }
    
    public void QuadDirectionalAttack()
    {
        Debug.Log("Quad directional attack");
        isAttacking = true;
        foreach (var fireSpot in fireSpots)
        {
            FireBulletFrom(fireSpot.transform, fireSpot.direction);
        }
        isAttacking = false;
    }

    public void DirectionAttack()
    {
        Debug.Log("Direction attack");
        isAttacking = true;
        foreach (var fireSpot in fireSpots)
        {
            Vector2 direction = (SeconfFrontTarget.position - fireSpot.transform.position).normalized;
            FireBulletFrom(fireSpot.transform, direction);
        }
        isAttacking = false;
    }
    
    
    
    private void FireBulletFrom(Transform fireSpot, Vector2 direction)
    {
        var bullet = Instantiate(bulletPrefab, fireSpot.position, fireSpot.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
    }
}