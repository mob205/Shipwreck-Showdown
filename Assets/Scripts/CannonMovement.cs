using Mirror;
using UnityEngine;

public class CannonMovement : NetworkBehaviour, IControllable
{
    [Header("Firing")]
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Projectile _cannonPrefab;
    [SerializeField] private int _damage;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _projectileSpeed;

    [Header("Cannon controls")]
    [SerializeField] private Transform _followTarget;
    [SerializeField] float _turnRate;
    [SerializeField] private float _angleClamp = 30;

    bool _canFire = true;

    [field: SerializeField] public bool RequiresAuthority { get; } = false;
    [field: SerializeField] public Transform CameraAngle { get; private set; }

    public GameObject Object { get { return gameObject; } }


    private float _rotInput;
    private float _rotOffset;

    private void Update()
    {
        if (!_followTarget) { return; }
        if (NetworkServer.active)
        {
            if (_rotInput != 0)
            {
                _rotOffset += Mathf.Sign(_rotInput) * _turnRate * Time.deltaTime;
                _rotOffset = Mathf.Clamp(_rotOffset, -_angleClamp, _angleClamp);
            }
            transform.rotation = Quaternion.Euler(0, 0, _followTarget.rotation.eulerAngles.z + _rotOffset);
        }
        
        transform.position = _followTarget.position;
    }

    [Command(requiresAuthority = false)]
    private void CmdRotateCannon(float xInput)
    {
        _rotInput = -xInput;
    }

    public void Fire()
    {
        CmdFire();
    }

    [Command(requiresAuthority = false)]
    public void CmdFire()
    {
        if(!_canFire) { return; }
        ShootCannon();
        
    }

    private void ShootCannon()
    {
        var cannonball = Instantiate(_cannonPrefab, _bulletSpawn.position, _bulletSpawn.transform.rotation);
        cannonball.Damage = _damage;
        cannonball.GetComponent<Rigidbody2D>().velocity = _bulletSpawn.forward * _projectileSpeed;
    }

    public void Move(Vector2 input)
    {
        CmdRotateCannon(input.x);
    }

    public void OnReleaseControl()
    {
        CmdRotateCannon(0);
    }
}
