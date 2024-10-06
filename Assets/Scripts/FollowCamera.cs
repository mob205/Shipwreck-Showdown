using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private PlayerController _player;
    private float _startZ;
    private Camera _camera;

    void Start()
    {
        _player = PlayerController.LocalController;
        _startZ = transform.position.z;
        _camera = GetComponent<Camera>();
        if(NetworkServer.active && !NetworkClient.active)
        {
            Destroy(gameObject); // don't need camera on server
        }
    }

    private void Update()
    {
        if(_player != null)
        {
            var cameraPos = _player.CurrentControllable.CameraAngle.position;
            transform.position = new Vector3(cameraPos.x, cameraPos.y, _startZ);
            _camera.orthographicSize = _player.CurrentControllable.CameraSize;
        }
        else
        {
            _player = PlayerController.LocalController;
        }
    }
}
