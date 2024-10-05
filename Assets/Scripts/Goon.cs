using UnityEngine;

public class Goon : MonoBehaviour {
    [SerializeField] private float speed;
    private Rigidbody2D _rb;
    private PlayerMovement _targetPlayer;
    private BoxCollider2D _collider;

    // Function to initialize the enemy
    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Function to update the enemy
    private void Update() {
        FindTargetPlayer();
        MoveTowardsTarget();
    }

    // Function to find the target player
    private void FindTargetPlayer() {
        PlayerMovement[] players = GameObject.FindObjectsOfType<PlayerMovement>();

        // TODO: Implement checking if any player is the captain
        // Loop through all players to find the captain
        foreach (var player in players) {
            // TODO: Replace 'player.isCaptain' with the actual property
            if (/* player.isCaptain */ false) /* Placeholder condition */ {
                _targetPlayer = player;
                return;
            }
        }

        // If no captain is found, find the closest player
        float closestDistance = Mathf.Infinity;
        foreach (var player in players) {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance) {
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
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Enemy collided with player");
        }
    }
}