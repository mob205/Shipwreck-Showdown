using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.BouncyCastle.Tls;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour, IControllable
{
    [SerializeField] private float _deceleration;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;

    [field: SerializeField] public Transform CameraAngle { get; private set; }
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
        _rb.velocity = _frameVelocity;
        if (_frameInput == Vector2.zero)
        {
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity = Vector2.MoveTowards(_frameVelocity, _frameInput * _maxSpeed, _acceleration * Time.fixedDeltaTime);
        }
    }
    public void OnReleaseControl()
    {
        _frameInput = Vector2.zero;
    }

    public void Fire()
    {
        Debug.Log("Swinging sword");
    }

    public void Move(Vector2 input)
    {
        _frameInput = input;
    }
}
