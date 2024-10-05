using System.Collections;
using System.Collections.Generic;
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
            Debug.Log("Null :(");
            _player = PlayerController.LocalController;
        }
    }
}
