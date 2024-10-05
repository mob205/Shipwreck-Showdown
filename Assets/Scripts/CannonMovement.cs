using Mirror;
using UnityEngine;

public class CannonMovement : NetworkBehaviour, IControllable
{
    [field: SerializeField] public Transform CameraAngle { get; private set; }

    public GameObject Object => throw new System.NotImplementedException();

    public void Fire()
    {
        throw new System.NotImplementedException();
    }

    public void Move(Vector2 input)
    {
        throw new System.NotImplementedException();
    }

    public void OnReleaseControl()
    {
        throw new System.NotImplementedException();
    }
}
