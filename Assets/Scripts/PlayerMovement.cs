using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour, IControllable
{
    [Header("Combat")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _attackRange;
    [SerializeField] private int _damage;
    [SerializeField] private float _fireRate;
   
    [Header("Movement")]
    [SerializeField] private float _deceleration;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;

    private bool _canFire = true;

    [field: SerializeField] public Transform CameraAngle { get; private set; }

    public bool RequiresAuthority { get; } = true;
    public GameObject Object { get { return gameObject; } }

    private Vector2 _frameVelocity;
    private Vector2 _frameInput;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_frameInput == Vector2.zero)
        {
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, _frameInput * _maxSpeed, _acceleration * Time.fixedDeltaTime);
        }
        _rb.velocity = _frameVelocity;
    }
    public void OnReleaseControl()
    {
        _frameInput = Vector2.zero;
    }

    public void Fire()
    {
        CmdFire();
    }

    [Command]
    private void CmdFire()
    {
        if(!_canFire) { return; }
        ProcessAttackDamage();
        _canFire = false;
        Invoke(nameof(EnableFire), 1f / _fireRate);
    }

    [Server]
    private void EnableFire()
    {
        _canFire = true;
    }

    [Server]
    private void ProcessAttackDamage()
    {
        var enemy = Physics2D.OverlapCircle(transform.position, _attackRange, _enemyLayer);
        if (enemy)
        {
            enemy.GetComponent<Health>().ModifyHealth(-_damage, gameObject);
        }
    }


    [ClientRpc]
    private void RpcSwingWeapon()
    {
        Debug.Log("Swing");
    }


    public void Move(Vector2 input)
    {
        _frameInput = input;
    }
}
