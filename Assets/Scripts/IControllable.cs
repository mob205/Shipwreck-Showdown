using UnityEngine;

public interface IControllable
{
    public void Move(Vector2 input);
    public void Fire();
    public void OnReleaseControl();

    public Transform CameraAngle { get; }

    public float CameraSize { get; }
    public bool RequiresAuthority { get; }
    public GameObject Object { get; }
}
