using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; }

    [field: SyncVar] public int CurrentHealth { get; private set; }

    public UnityEvent<Health> OnDeath;

    public UnityEvent<Health, int, GameObject> OnDamage;

    private bool _hasDied;

    public override void OnStartServer()
    {
        CurrentHealth = MaxHealth;
    }

    [Server]
    public void ModifyHealth(int amount, GameObject attacker)
    {
        CurrentHealth += amount;
        if(amount < 0)
        {
            OnDamage?.Invoke(this, amount, attacker);
        }
        if(CurrentHealth <= 0 && !_hasDied)
        {
            _hasDied = true;
            StartDeath();
        }
    }

    [Server]
    private void StartDeath()
    {
        OnDeath?.Invoke(this);
    }
}

