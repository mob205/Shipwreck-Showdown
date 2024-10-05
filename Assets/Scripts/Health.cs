using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; }

    [field: SyncVar] public int CurrentHealth { get; private set; }

    public UnityEvent<Health> OnDeath;

    public UnityEvent<Health, int> OnDamage;

    public override void OnStartServer()
    {
        CurrentHealth = MaxHealth;
    }

    [Server]
    public void ModifyHealth(int amount)
    {
        CurrentHealth += amount;
        if(amount < 0)
        {
            OnDamage?.Invoke(this, amount);
        }
        if(CurrentHealth <= 0)
        {
            StartDeath();
        }
    }

    [Server]
    private void StartDeath()
    {
        OnDeath?.Invoke(this);
        Debug.Log("Death on server!");
    }
}

