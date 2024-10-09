using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SyncVar] public int CurrentHealth { get; private set; }

    public bool IsVulnerable { get; set; } = true;

    public UnityEvent<Health> OnDeath;

    public UnityEvent<Health, int, GameObject> OnDamage;

    public bool HasDied { get; private set; }

    public override void OnStartServer()
    {
        CurrentHealth = MaxHealth;
    }

    [Server]
    public void ModifyHealth(int amount, GameObject attacker)
    {
        if (!IsVulnerable) { return; }
        CurrentHealth += amount;
        if(amount < 0)
        {
            OnDamage?.Invoke(this, amount, attacker);
        }
        if(CurrentHealth <= 0 && !HasDied)
        {
            HasDied = true;
            StartDeath();
        }
    }

    [Server]
    private void StartDeath()
    {
        OnDeath?.Invoke(this);
    }
}

