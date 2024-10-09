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
    [SerializeField] private AudioEvent sword;

    [SerializeField] private SpriteRenderer _swordIcon;
    [SerializeField] private float _swingSpeed;
    [SerializeField] private Rigidbody2D _swordRB;

   
    [Header("Movement")]
    [SerializeField] private float _deceleration;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;

    private bool _canFire = true;

    [field: SerializeField] public Transform CameraAngle { get; set; }
    [field: SerializeField] public float CameraSize { get; private set; }
    [field: SerializeField] public bool DoSway { get; private set; }

    public bool RequiresAuthority { get; } = true;
    public GameObject Object { get { return gameObject; } }

    private Vector2 _frameVelocity;
    private Vector2 _frameInput;
    private Rigidbody2D _rb;
    private AudioSource _source;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _source = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
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
        CmdOnInputChange(Vector2.zero);
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
        RpcSwingWeapon();
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
        sword.Play(_source);
        //_swordIcon.enabled = true;
        //_swordRB.angularVelocity = _swingSpeed;
        //Invoke(nameof(DisableSwordIcon), 360f / _swingSpeed);
        _animator.SetTrigger("Attack");
    }

    //private void DisableSwordIcon()
    //{
    //    _swordRB.angularVelocity = 0;
    //    _swordIcon.enabled = false;
    //}

    public void Move(Vector2 input)
    {
        _frameInput = input;
        CmdOnInputChange(input);
    }

    [ClientRpc]
    private void RpcOnInputChange(Vector2 input)
    {
        if (input.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (input.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        _animator.SetInteger("MoveX", Mathf.CeilToInt(input.x));
        _animator.SetInteger("MoveY", Mathf.CeilToInt(input.y));
    }

    [Command]
    private void CmdOnInputChange(Vector2 input)
    {
        RpcOnInputChange(input);
    }
}