using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 16.0f, -20.0f);

    [SerializeField]
    GameObject _target = null;

    public void SetPlayer(GameObject player)
    {
        _target = player;
    }

    void LateUpdate()
    {
        if (_target.IsValid() == false)
            return;

        transform.position = _target.transform.position + _delta;
        transform.LookAt(_target.transform);
    }
}
