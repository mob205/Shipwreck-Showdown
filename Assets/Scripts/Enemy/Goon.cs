using UnityEngine;
using Mirror;

public class Goon : NetworkBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    [SerializeField] private AudioEvent _deathAudio;
    [SerializeField] private AudioEvent _damageAudio;

    private Rigidbody2D _rb;
    private PlayerController _targetPlayer;
    private Health _health;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private float damageCooldown = 1f;
    private float damageTimer = 0f;

    [SerializeField] private float _damageFlashDuration;

    // Function to initialize the enemy
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        RpcOnDeath(transform.position);
        Invoke(nameof(CleanupGoon), 5);
    }

    private void CleanupGoon()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcOnDeath(Vector2 position)
    {
        if(_deathAudio)
        {
            _deathAudio.PlayOneShot(position);
        }
        _spriteRenderer.enabled = false;
    }
    private void OnDamage(Health health, int amount, GameObject attacker)
    {
        if(attacker.TryGetComponent(out PlayerController player))
        {
            _targetPlayer = player;
        }
        RpcOnDamage();
    }

    [ClientRpc]
    private void RpcOnDamage()
    {
        if(_damageAudio)
        {
            _damageAudio.Play(_audioSource);
        }
        _spriteRenderer.color = Color.red;
        Invoke(nameof(ResetColor), _damageFlashDuration);
    }

    private void ResetColor()
    {
        _spriteRenderer.color = Color.white;
    }

    // Function to update the enemy
    private void Update() {
        if(_health.HasDied) { return; }
        // Only retarget to the closest non-captain player if there's not already a target
        if(_targetPlayer == null || _targetPlayer.GetComponent<Health>().HasDied)
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
            if(player.GetComponent<Health>().HasDied) { continue; }
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
        if (other.gameObject.tag == "Player" && NetworkServer.active && !_health.HasDied) {
            if (damageTimer <= 0) 
            {
                other.gameObject.GetComponent<Health>().ModifyHealth(-damage, gameObject);
                
                damageTimer = damageCooldown;
            }
            
        }
    }
}