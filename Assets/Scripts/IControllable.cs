using UnityEngine;

public interface IControllable
{
    public void Move(Vector2 input);
    public void Fire();
    public Transform CameraAngle { get; }

    public GameObject Object { get; }
}
