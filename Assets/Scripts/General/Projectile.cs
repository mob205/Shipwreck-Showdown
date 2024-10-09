using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject Shooter { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [SerializeField] private LayerMask _attackLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_attackLayer & (1 << collision.gameObject.layer)) != 0 && collision.TryGetComponent(out Health health))
        {
            if (NetworkServer.active)
            {
                health.ModifyHealth(-Damage, Shooter);
            }
            Destroy(gameObject);
        }
    }
}
