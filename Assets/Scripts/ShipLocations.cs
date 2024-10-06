using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLocations : MonoBehaviour
{
    [SerializeField] private Transform _front;
    [SerializeField] private Transform _back;
    [SerializeField] private Transform _closeFront;
    [SerializeField] private Transform _center;

    public static Transform Front { get; private set; }
    public static Transform Back { get; private set; }
    public static Transform CloseFront { get; private set; }
    public static Transform Center { get; private set; }

    private void Awake()
    {
        Front = _front;
        Back = _back;
        CloseFront = _closeFront;
        Center = _center;
    }
}
