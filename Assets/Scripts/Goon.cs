using UnityEngine;
using Mirror;

public class Goon : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    private Rigidbody2D _rb;
    private PlayerController _targetPlayer;
    private Health _health;

    [SerializeField] private float damageCooldown = 1f; 
    private float damageTimer = 0f;

    // Function to initialize the enemy
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
    }

    private void Start()
    {
        if (!NetworkServer.active)
        {
            Destroy(this);
            return;
        }
        _health.OnDamage.AddListener(OnDamage);
        _health.OnDeath.AddListener(OnDeath);

        ControlInteractor.OnPossessed.AddListener(OnNearbyPossession);
    }
    private void OnNearbyPossession(ControlInteractor interactor)
    {
        _targetPlayer = interactor.BoundController;
    }
    private void OnDeath(Health health)
    {
        ControlInteractor.OnPossessed.RemoveListener(OnNearbyPossession);
        NetworkServer.Destroy(gameObject);
    }
    private void OnDamage(Health health, int amount, GameObject attacker)
    {
        if(attacker.TryGetComponent(out PlayerController player))
        {
            _targetPlayer = player;
        }
    }

    // Function to update the enemy
    private void Update() {
        // Only retarget to the closest non-captain player if there's not already a target
        if(_targetPlayer == null)
        {
            FindTargetPlayer();
        }
        
        MoveTowardsTarget();

        if (damageTimer > 0) {
            damageTimer -= Time.deltaTime;
        }
    }

    // Function to find the target player
    private void FindTargetPlayer() {
        PlayerController[] players = FindObjectsOfType<PlayerController>();

        // TODO: Implement checking if any player is the captain
        // Loop through all players to find the captain
        foreach (var player in players) {
            // TODO: Replace 'player.isCaptain' with the actual property
            if (player.IsCaptain)
            {
                _targetPlayer = player;
                return;
            }
        }

        // If no captain is found, find the closest player
        float closestDistance = Mathf.Infinity;

        foreach (var player in players) {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance) 
            {
                closestDistance = distance;
                _targetPlayer = player;
            }
        }
    }

    // Function to move the enemy towards the target player
    private void MoveTowardsTarget() {
        if (_targetPlayer != null) {
            Vector2 direction = (_targetPlayer.transform.position - transform.position).normalized;
            _rb.velocity = direction * speed;

            // TODO: If chasing the captain, check if they are still the captain
            // If not, reset the target player to find a new target
            // if (_targetPlayer.isCaptain == false) { _targetPlayer = null; }
        }
        else {
            _rb.velocity = Vector2.zero;
        }
    }
    
    // Function to check if the enemy is colliding with the player
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.tag == "Player" && NetworkServer.active) {
            if (damageTimer <= 0) 
            {
                other.gameObject.GetComponent<Health>().ModifyHealth(-damage, gameObject);
                
                damageTimer = damageCooldown;
            }
            
        }
    }
}