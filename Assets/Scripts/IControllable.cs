using UnityEngine;

public interface IControllable
{
    public void Move(Vector2 input);
    public void Fire();
    public void OnReleaseControl();

    public Transform CameraAngle { get; set; }

    public float CameraSize { get; }
    public bool RequiresAuthority { get; }

    public bool DoSway { get; }
    public GameObject Object { get; }
}
