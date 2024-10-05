using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private PlayerController _player;
    private float _startZ;

    void Start()
    {
        _player = PlayerController.LocalController;
        _startZ = transform.position.z;
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
        }
        else
        {
            _player = PlayerController.LocalController;
        }
    }
}
