using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLocations : MonoBehaviour
{
    [SerializeField] private Transform _front;
    [SerializeField] private Transform _back;

    public static Transform Front { get; private set; }
    public static Transform Back { get; private set; }

    private void Awake()
    {
        Front = _front;
        Back = _back;
    }
}
