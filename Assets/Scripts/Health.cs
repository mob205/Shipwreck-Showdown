using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }

    [field: SyncVar] public float CurrentHealth { get; private set; }

    public UnityEvent<Health> OnDeath;

    [Server]
    public void ModifyHealth(float amount)
    {
        CurrentHealth -= amount;
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

