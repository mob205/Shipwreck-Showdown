using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMovement : MonoBehaviour, IControllable
{
    [SerializeField] private float _deceleration;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _turnRate;

    [field: SerializeField] public Transform CameraAngle { get; set; }
    [field: SerializeField] public float CameraSize { get; private set; }
    [field: SerializeField] public bool DoSway { get; } = false;
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
        if(_frameInput.y != 0)
        {
            _frameInput.y = Mathf.Sign(_frameInput.y);
        }
        if(_frameInput.x != 0)
        {
            _frameInput.x = Mathf.Sign(_frameInput.x);
        }

        if (_frameInput == Vector2.zero)
        {
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, _frameInput * _maxSpeed, _acceleration * Time.fixedDeltaTime);
        }
        _rb.velocity = transform.rotation * new Vector3(_frameVelocity.y, 0, 0);

        // negate input to make the positive input, which is to the right, correspond to a right (CW) rotation
        _rb.angularVelocity = -_frameInput.x * _turnRate/* * Mathf.Sign(_frameInput.y)*/; // Uncomment sign to have car-like reverse steering
    }

    public void Fire()
    {
        // nothing 
    }

    public void Move(Vector2 input)
    {
        _frameInput = input;
    }
    public void OnReleaseControl()
    {
        _frameInput = Vector2.zero;
    }
}
