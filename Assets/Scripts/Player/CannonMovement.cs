using Mirror;
using UnityEngine;

public class CannonMovement : NetworkBehaviour, IControllable
{
    [Header("Firing")]
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Projectile _cannonPrefab;
    [SerializeField] private AudioEvent _clip;
    [SerializeField] private int _damage;
    [SerializeField] private float _projectileSpeed;

    [Header("Cannon controls")]
    [SerializeField] private Transform _followTarget;
    [SerializeField] float _turnRate;
    [SerializeField] private float _angleClamp = 30;

    //private int _numCannonballsLoaded = 0;
    public bool HasCannonballLoaded { get; private set; }

    [field: SerializeField] public bool RequiresAuthority { get; } = false;
    [field: SerializeField] public Transform CameraAngle { get; set; }
    [field: SerializeField] public float CameraSize { get; private set; }
    [field: SerializeField] public bool DoSway { get; } = false;

    public GameObject Object { get { return gameObject; } }


    private float _rotInput;
    private float _rotOffset;

    private AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }
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
    private void CmdFire()
    {
        if(!HasCannonballLoaded) { return; }
        HasCannonballLoaded = false;

        ShootCannon();
        RpcSimulateFire();
    }

    [ClientRpc]
    private void RpcSimulateFire()
    {
        // Filter out hosts
        if(!NetworkServer.active)
        {
            ShootCannon();
        }
        _clip.Play(_source);
    }
    
    public void LoadCannon()
    {
        HasCannonballLoaded = true;
    }

    private void ShootCannon()
    {
        var cannonball = Instantiate(_cannonPrefab, _bulletSpawn.position, _bulletSpawn.transform.rotation);
        cannonball.Damage = _damage;
        cannonball.GetComponent<Rigidbody2D>().velocity = _bulletSpawn.right * _projectileSpeed;
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
